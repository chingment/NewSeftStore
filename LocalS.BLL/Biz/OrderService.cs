﻿using LocalS.BLL;
using LocalS.BLL.Task;
using LocalS.Entity;
using LocalS.BLL.Mq;
using LocalS.BLL.Mq.MqByRedis;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using TongGuanPaySdk;

namespace LocalS.BLL.Biz
{
    public class OrderService : BaseDbContext
    {

        private static readonly object lock_Reserve = new object();

        public CustomJsonResult<RetOrderReserve> Reserve(string operater, RopOrderReserve rop)
        {
            CustomJsonResult<RetOrderReserve> result = new CustomJsonResult<RetOrderReserve>();

            lock (lock_Reserve)
            {
                if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定商品为空", null);
                }

                var store = BizFactory.Store.GetOne(rop.StoreId);

                if (store == null)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定店铺无效", null);
                }

                if (!store.IsOpen)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "店铺已暂停营业", null);
                }

                List<string> machineIdsByValid = new List<string>();

                foreach (var sellChannelRefId in rop.SellChannelRefIds)
                {
                    var machine = BizFactory.Machine.GetOne(sellChannelRefId);
                    if (machine.RunStatus == E_MachineRunStatus.Running)
                    {
                        machineIdsByValid.Add(machine.Id);
                    }
                }

                if (machineIdsByValid.Count == 0)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "该店铺未配置售货机", null);
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    RetOrderReserve ret = new RetOrderReserve();

                    List<ProductSkuInfoAndStockModel> bizProductSkus = new List<BLL.ProductSkuInfoAndStockModel>();

                    #region 检查可售商品信息是否符合实际环境
                    List<string> warn_tips = new List<string>();

                    foreach (var productSku in rop.ProductSkus)
                    {
                        var bizProductSku = CacheServiceFactory.ProductSku.GetInfoAndStock(store.MerchId, rop.StoreId, machineIdsByValid.ToArray(), productSku.Id);

                        if (bizProductSku == null)
                        {
                            warn_tips.Add(string.Format("{0}商品信息不存在", bizProductSku.Name));
                        }
                        else
                        {
                            if (bizProductSku.Stocks.Count == 0)
                            {
                                warn_tips.Add(string.Format("{0}商品库存信息不存在", bizProductSku.Name));
                            }
                            else
                            {
                                var sellQuantity = bizProductSku.Stocks.Sum(m => m.SellQuantity);

                                Console.WriteLine("sellQuantity：" + sellQuantity);

                                if (bizProductSku.Stocks[0].IsOffSell)
                                {
                                    warn_tips.Add(string.Format("{0}已经下架", bizProductSku.Name));
                                }
                                else
                                {
                                    if (sellQuantity < productSku.Quantity)
                                    {
                                        warn_tips.Add(string.Format("{0}的可销售数量为{1}个", bizProductSku.Name, sellQuantity));
                                    }
                                    else
                                    {
                                        bizProductSkus.Add(bizProductSku);
                                    }
                                }
                            }
                        }
                    }

                    if (warn_tips.Count > 0)
                    {
                        return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, string.Join(";", warn_tips.ToArray()), null);
                    }

                    #endregion

                    var reserveDetails = GetReserveDetail(rop.ProductSkus, bizProductSkus);


                    var order = new Order();
                    order.Id = GuidUtil.New();
                    order.Sn = RedisSnUtil.Build(RedisSnType.Order, store.MerchId);
                    order.MerchId = store.MerchId;
                    order.StoreId = rop.StoreId;
                    order.StoreName = store.Name;
                    order.ClientUserId = rop.ClientUserId;
                    order.ClientUserName = BizFactory.Merch.GetClientName(order.MerchId, rop.ClientUserId);
                    order.Quantity = rop.ProductSkus.Sum(m => m.Quantity);
                    order.Status = E_OrderStatus.WaitPay;
                    order.Source = rop.Source;
                    order.PickCode = RedisSnUtil.BuildPickCode();

                    if (order.PickCode == null)
                    {
                        return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定下单生成取货码失败", null);
                    }

                    order.SubmitTime = DateTime.Now;
                    order.PayExpireTime = DateTime.Now.AddSeconds(300);
                    order.Creator = operater;
                    order.CreateTime = DateTime.Now;

                    #region 更改购物车标识

                    if (!string.IsNullOrEmpty(rop.ClientUserId))
                    {
                        var cartsIds = rop.ProductSkus.Select(m => m.CartId).Distinct().ToArray();
                        if (cartsIds != null)
                        {
                            var clientCarts = CurrentDb.ClientCart.Where(m => cartsIds.Contains(m.Id) && m.ClientUserId == rop.ClientUserId).ToList();
                            if (clientCarts != null)
                            {
                                foreach (var cart in clientCarts)
                                {
                                    cart.Status = E_ClientCartStatus.Settling;
                                    cart.Mender = operater;
                                    cart.MendTime = DateTime.Now;
                                    CurrentDb.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion

                    order.OriginalAmount = reserveDetails.Sum(m => m.OriginalAmount);
                    order.DiscountAmount = reserveDetails.Sum(m => m.DiscountAmount);
                    order.ChargeAmount = reserveDetails.Sum(m => m.ChargeAmount);
                    order.SellChannelRefIds = string.Join(",", reserveDetails.Select(m => m.SellChannelRefId).ToArray());

                    foreach (var detail in reserveDetails)
                    {
                        var orderDetails = new OrderDetails();
                        orderDetails.Id = GuidUtil.New();
                        orderDetails.Sn = order.Sn + reserveDetails.IndexOf(detail).ToString();
                        orderDetails.ClientUserId = rop.ClientUserId;
                        orderDetails.MerchId = store.MerchId;
                        orderDetails.StoreId = rop.StoreId;
                        orderDetails.SellChannelRefType = detail.SellChannelRefType;
                        orderDetails.SellChannelRefId = detail.SellChannelRefId;
                        switch (detail.SellChannelRefType)
                        {
                            //case E_SellChannelRefType.Express:
                            //    orderDetails.SellChannelRefName = "【快递】";
                            //    orderDetails.SellChannelRefType = E_SellChannelRefType.Express;
                            //    orderDetails.SellChannelRefId = GuidUtil.Empty();
                            //    orderDetails.Receiver = rop.Receiver;
                            //    orderDetails.ReceiverPhone = rop.ReceiverPhone;
                            //    orderDetails.ReceptionAddress = rop.ReceptionAddress;
                            //    break;
                            //case E_SellChannelRefType.SelfTake:
                            //    orderDetails.SellChannelRefName = "【店内自取】";
                            //    orderDetails.SellChannelRefType = E_SellChannelRefType.SelfTake;
                            //    orderDetails.SellChannelRefId = GuidUtil.Empty();
                            //    orderDetails.Receiver = rop.Receiver;
                            //    orderDetails.ReceiverPhone = rop.ReceiverPhone;
                            //    orderDetails.ReceptionAddress = rop.ReceptionAddress;
                            //    break;
                            case E_SellChannelRefType.Machine:
                                orderDetails.SellChannelRefName = "【机器自提】 " + BizFactory.Merch.GetMachineName(order.MerchId, detail.SellChannelRefId);
                                orderDetails.SellChannelRefType = E_SellChannelRefType.Machine;
                                orderDetails.SellChannelRefId = detail.SellChannelRefId;
                                orderDetails.Receiver = null;
                                orderDetails.ReceiverPhone = null;
                                orderDetails.ReceptionAddress = store.Address;
                                break;
                        }
                        orderDetails.OrderId = order.Id;
                        orderDetails.OrderSn = order.Sn;
                        orderDetails.OriginalAmount = detail.OriginalAmount;
                        orderDetails.DiscountAmount = detail.DiscountAmount;
                        orderDetails.ChargeAmount = detail.ChargeAmount;
                        orderDetails.Status = E_OrderStatus.WaitPay;
                        orderDetails.Quantity = detail.Quantity;
                        orderDetails.SubmitTime = DateTime.Now;
                        orderDetails.Creator = operater;
                        orderDetails.CreateTime = DateTime.Now;
                        CurrentDb.OrderDetails.Add(orderDetails);

                        foreach (var detailsChild in detail.Details)
                        {
                            var orderDetailsChild = new OrderDetailsChild();
                            orderDetailsChild.Id = GuidUtil.New();
                            orderDetailsChild.Sn = orderDetails.Sn + detail.Details.IndexOf(detailsChild).ToString();
                            orderDetailsChild.ClientUserId = rop.ClientUserId;
                            orderDetailsChild.MerchId = store.MerchId;
                            orderDetailsChild.StoreId = rop.StoreId;
                            orderDetailsChild.SellChannelRefType = detailsChild.SellChannelRefType;
                            orderDetailsChild.SellChannelRefId = detailsChild.SellChannelRefId;
                            orderDetailsChild.SellChannelRefName = orderDetails.SellChannelRefName;
                            orderDetailsChild.OrderId = order.Id;
                            orderDetailsChild.OrderSn = order.Sn;
                            orderDetailsChild.OrderDetailsId = orderDetails.Id;
                            orderDetailsChild.OrderDetailsSn = orderDetails.Sn;
                            orderDetailsChild.PrdProductSkuId = detailsChild.ProductSkuId;
                            orderDetailsChild.PrdProductId = detailsChild.ProductId;
                            orderDetailsChild.PrdProductSkuName = detailsChild.ProductSkuName;
                            orderDetailsChild.PrdProductSkuMainImgUrl = detailsChild.ProductSkuMainImgUrl;
                            orderDetailsChild.SalePrice = detailsChild.SalePrice;
                            orderDetailsChild.SalePriceByVip = detailsChild.SalePriceByVip;
                            orderDetailsChild.Quantity = detailsChild.Quantity;
                            orderDetailsChild.OriginalAmount = detailsChild.OriginalAmount;
                            orderDetailsChild.DiscountAmount = detailsChild.DiscountAmount;
                            orderDetailsChild.ChargeAmount = detailsChild.ChargeAmount;
                            orderDetailsChild.SubmitTime = DateTime.Now;
                            orderDetailsChild.Status = E_OrderStatus.WaitPay;
                            orderDetailsChild.Creator = operater;
                            orderDetailsChild.CreateTime = DateTime.Now;
                            CurrentDb.OrderDetailsChild.Add(orderDetailsChild);

                            foreach (var detailsChildSon in detailsChild.Details)
                            {
                                var orderDetailsChildSon = new OrderDetailsChildSon();
                                orderDetailsChildSon.Id = GuidUtil.New();
                                orderDetailsChildSon.Sn = orderDetailsChild.Sn + detailsChild.Details.IndexOf(detailsChildSon);
                                orderDetailsChildSon.ClientUserId = rop.ClientUserId;
                                orderDetailsChildSon.MerchId = store.MerchId;
                                orderDetailsChildSon.StoreId = rop.StoreId;
                                orderDetailsChildSon.SellChannelRefType = detailsChildSon.SellChannelRefType;
                                orderDetailsChildSon.SellChannelRefId = detailsChildSon.SellChannelRefId;
                                orderDetailsChildSon.SellChannelRefName = orderDetailsChild.SellChannelRefName;
                                orderDetailsChildSon.OrderId = order.Id;
                                orderDetailsChildSon.OrderSn = order.Sn;
                                orderDetailsChildSon.OrderDetailsId = orderDetails.Id;
                                orderDetailsChildSon.OrderDetailsSn = orderDetails.Sn;
                                orderDetailsChildSon.OrderDetailsChildId = orderDetailsChild.Id;
                                orderDetailsChildSon.OrderDetailsChildSn = orderDetailsChild.Sn;
                                orderDetailsChildSon.SlotId = detailsChildSon.SlotId;
                                orderDetailsChildSon.PrdProductSkuId = detailsChildSon.ProductSkuId;
                                orderDetailsChildSon.PrdProductId = orderDetailsChild.PrdProductId;
                                orderDetailsChildSon.PrdProductSkuName = detailsChildSon.ProductSkuName;
                                orderDetailsChildSon.PrdProductSkuMainImgUrl = detailsChildSon.ProductSkuMainImgUrl;
                                orderDetailsChildSon.SalePrice = detailsChildSon.SalePrice;
                                orderDetailsChildSon.SalePriceByVip = detailsChildSon.SalePriceByVip;
                                orderDetailsChildSon.Quantity = detailsChildSon.Quantity;
                                orderDetailsChildSon.OriginalAmount = detailsChildSon.OriginalAmount;
                                orderDetailsChildSon.DiscountAmount = detailsChildSon.DiscountAmount;
                                orderDetailsChildSon.ChargeAmount = detailsChildSon.ChargeAmount;
                                orderDetailsChildSon.SubmitTime = DateTime.Now;
                                orderDetailsChildSon.Creator = operater;
                                orderDetailsChildSon.CreateTime = DateTime.Now;
                                orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.WaitPay;
                                CurrentDb.OrderDetailsChildSon.Add(orderDetailsChildSon);
                            }

                            foreach (var slotStock in detailsChild.SlotStock)
                            {
                                BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderReserve, order.MerchId, order.StoreId, slotStock.SellChannelRefId, slotStock.SlotId, slotStock.ProductSkuId, slotStock.Quantity);
                            }
                        }
                    }

                    CurrentDb.Order.Add(order);
                    CurrentDb.SaveChanges();
                    ts.Complete();


                    Task4Factory.Global.Enter(Task4TimType.Order2CheckPay, order.Id, order.PayExpireTime.Value, order);

                    ret.OrderId = order.Id;
                    ret.OrderSn = order.Sn;
                    ret.ChargeAmount = order.ChargeAmount.ToF2Price();

                    result = new CustomJsonResult<RetOrderReserve>(ResultType.Success, ResultCode.Success, "预定成功", ret);

                }
            }

            return result;

        }

        private List<OrderReserveDetail> GetReserveDetail(List<RopOrderReserve.ProductSku> reserveDetails, List<ProductSkuInfoAndStockModel> productSkus)
        {
            List<OrderReserveDetail> details = new List<OrderReserveDetail>();

            List<OrderReserveDetail.DetailChildSon> detailChildSons = new List<OrderReserveDetail.DetailChildSon>();

            var receptionModes = reserveDetails.Select(m => m.ReceptionMode).Distinct().ToArray();

            foreach (var receptionMode in receptionModes)
            {
                var l_reserveDetails = reserveDetails.Where(m => m.ReceptionMode == receptionMode).ToList();

                foreach (var reserveDetail in l_reserveDetails)
                {
                    E_SellChannelRefType channelType = E_SellChannelRefType.Unknow;
                    switch (receptionMode)
                    {
                        //case E_ReceptionMode.Express:
                        //    channelType = E_SellChannelRefType.Express;
                        //    break;
                        //case E_ReceptionMode.SelfTake:
                        //    channelType = E_SellChannelRefType.SelfTake;
                        //    break;
                        case E_ReceptionMode.Machine:
                            channelType = E_SellChannelRefType.Machine;
                            break;

                    }
                    var productSku = productSkus.Where(m => m.Id == reserveDetail.Id).FirstOrDefault();

                    var productSku_Stocks = productSku.Stocks.Where(m => m.RefType == channelType).ToList();

                    foreach (var item in productSku_Stocks)
                    {
                        for (var i = 0; i < item.SellQuantity; i++)
                        {
                            int reservedQuantity = detailChildSons.Where(m => m.ProductSkuId == reserveDetail.Id && m.SellChannelRefType == channelType).Sum(m => m.Quantity);//已订的数量
                            int needReserveQuantity = reserveDetail.Quantity;//需要订的数量
                            if (reservedQuantity != needReserveQuantity)
                            {
                                var detailChildSon = new OrderReserveDetail.DetailChildSon();
                                detailChildSon.Id = GuidUtil.New();
                                detailChildSon.SellChannelRefType = item.RefType;
                                detailChildSon.SellChannelRefId = item.RefId;
                                detailChildSon.ReceptionMode = receptionMode;
                                detailChildSon.ProductSkuId = productSku.Id;
                                detailChildSon.ProductId = productSku.ProductId;
                                detailChildSon.ProductSkuName = productSku.Name;
                                detailChildSon.ProductSkuMainImgUrl = productSku.MainImgUrl;
                                detailChildSon.SlotId = item.SlotId;
                                detailChildSon.Quantity = 1;
                                detailChildSon.SalePrice = productSku_Stocks[0].SalePrice;
                                detailChildSon.SalePriceByVip = productSku_Stocks[0].SalePriceByVip;
                                detailChildSon.OriginalAmount = detailChildSon.Quantity * productSku_Stocks[0].SalePrice;
                                detailChildSons.Add(detailChildSon);
                            }
                        }
                    }
                }
            }

            var sumOriginalAmount = detailChildSons.Sum(m => m.OriginalAmount);
            var sumDiscountAmount = 0m;
            for (int i = 0; i < detailChildSons.Count; i++)
            {
                decimal scale = (sumOriginalAmount == 0 ? 0 : (detailChildSons[i].OriginalAmount / sumOriginalAmount));
                detailChildSons[i].DiscountAmount = Decimal.Round(scale * sumDiscountAmount, 2);
                detailChildSons[i].ChargeAmount = detailChildSons[i].OriginalAmount - detailChildSons[i].DiscountAmount;
            }

            var sumDiscountAmount2 = detailChildSons.Sum(m => m.DiscountAmount);
            if (sumDiscountAmount != sumDiscountAmount2)
            {
                var diff = sumDiscountAmount - sumDiscountAmount2;

                detailChildSons[detailChildSons.Count - 1].DiscountAmount = detailChildSons[detailChildSons.Count - 1].DiscountAmount + diff;
                detailChildSons[detailChildSons.Count - 1].ChargeAmount = detailChildSons[detailChildSons.Count - 1].OriginalAmount - detailChildSons[detailChildSons.Count - 1].DiscountAmount;
            }


            var detailGroups = (from c in detailChildSons
                                select new
                                {
                                    c.ReceptionMode,
                                    c.SellChannelRefType,
                                    c.SellChannelRefId
                                }).Distinct().ToList();



            foreach (var detailGroup in detailGroups)
            {
                var detail = new OrderReserveDetail();
                detail.ReceptionMode = detailGroup.ReceptionMode;
                detail.SellChannelRefType = detailGroup.SellChannelRefType;
                detail.SellChannelRefId = detailGroup.SellChannelRefId;
                detail.Quantity = detailChildSons.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.Quantity);
                detail.OriginalAmount = detailChildSons.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.OriginalAmount);
                detail.DiscountAmount = detailChildSons.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.DiscountAmount);
                detail.ChargeAmount = detailChildSons.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.ChargeAmount);


                var detailChildGroups = (from c in detailChildSons
                                         where c.SellChannelRefId == detailGroup.SellChannelRefId
                                         select new
                                         {
                                             c.SellChannelRefType,
                                             c.SellChannelRefId,
                                             c.ProductSkuId
                                         }).Distinct().ToList();

                foreach (var detailChildGroup in detailChildGroups)
                {

                    var detailChild = new OrderReserveDetail.DetailChild();
                    detailChild.SellChannelRefType = detailChildGroup.SellChannelRefType;
                    detailChild.SellChannelRefId = detailChildGroup.SellChannelRefId;
                    detailChild.ProductSkuId = detailChildGroup.ProductSkuId;
                    detailChild.ProductId = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().ProductId;
                    detailChild.ProductSkuName = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().ProductSkuName;
                    detailChild.ProductSkuMainImgUrl = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().ProductSkuMainImgUrl;
                    detailChild.SalePrice = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().SalePrice;
                    detailChild.SalePriceByVip = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().SalePriceByVip;
                    detailChild.Quantity = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.Quantity);
                    detailChild.OriginalAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.OriginalAmount);
                    detailChild.DiscountAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.DiscountAmount);
                    detailChild.ChargeAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.ChargeAmount);

                    var detailChildSonGroups = (from c in detailChildSons
                                                where c.SellChannelRefId == detailChildGroup.SellChannelRefId
                                             && c.ProductSkuId == detailChildGroup.ProductSkuId
                                                select new
                                                {
                                                    c.Id,
                                                    c.ReceptionMode,
                                                    c.SellChannelRefType,
                                                    c.SellChannelRefId,
                                                    c.ProductSkuId,
                                                    c.SlotId,
                                                    c.Quantity,
                                                    c.SalePrice,
                                                    c.SalePriceByVip,
                                                    c.ProductSkuMainImgUrl,
                                                    c.ProductSkuName,
                                                    c.ChargeAmount,
                                                    c.DiscountAmount,
                                                    c.OriginalAmount
                                                }).ToList();


                    foreach (var detailChildSonGroup in detailChildSonGroups)
                    {
                        var detailChildSon = new OrderReserveDetail.DetailChildSon();
                        detailChildSon.Id = detailChildSonGroup.Id;
                        detailChildSon.SellChannelRefType = detailChildSonGroup.SellChannelRefType;
                        detailChildSon.SellChannelRefId = detailChildSonGroup.SellChannelRefId;
                        detailChildSon.ReceptionMode = detailChildSonGroup.ReceptionMode;
                        detailChildSon.ProductSkuId = detailChildSonGroup.ProductSkuId;
                        detailChildSon.SlotId = detailChildSonGroup.SlotId;
                        detailChildSon.Quantity = detailChildSonGroup.Quantity;
                        detailChildSon.ProductSkuName = detailChildSonGroup.ProductSkuName;
                        detailChildSon.ProductSkuMainImgUrl = detailChildSonGroup.ProductSkuMainImgUrl;
                        detailChildSon.SalePrice = detailChildSonGroup.SalePrice;
                        detailChildSon.SalePriceByVip = detailChildSonGroup.SalePriceByVip;
                        detailChildSon.OriginalAmount = detailChildSonGroup.OriginalAmount;
                        detailChildSon.DiscountAmount = detailChildSonGroup.DiscountAmount;
                        detailChildSon.ChargeAmount = detailChildSonGroup.ChargeAmount;
                        detailChild.Details.Add(detailChildSon);
                    }



                    var slotStockGroups = (from c in detailChildSons
                                           where c.SellChannelRefId == detailChildGroup.SellChannelRefId
                                        && c.ProductSkuId == detailChildGroup.ProductSkuId
                                           select new
                                           {
                                               c.SellChannelRefType,
                                               c.SellChannelRefId,
                                               c.ProductSkuId,
                                               c.SlotId
                                           }).Distinct().ToList();


                    foreach (var slotStockGroup in slotStockGroups)
                    {
                        var slotStock = new OrderReserveDetail.SlotStock();
                        slotStock.SellChannelRefType = slotStockGroup.SellChannelRefType;
                        slotStock.SellChannelRefId = slotStockGroup.SellChannelRefId;
                        slotStock.ProductSkuId = slotStockGroup.ProductSkuId;
                        slotStock.SlotId = slotStockGroup.SlotId;
                        slotStock.Quantity = detailChildSons.Where(m => m.SellChannelRefType == slotStockGroup.SellChannelRefType && m.SellChannelRefId == slotStockGroup.SellChannelRefId && m.ProductSkuId == slotStockGroup.ProductSkuId && m.SlotId == slotStockGroup.SlotId).Sum(m => m.Quantity);
                        detailChild.SlotStock.Add(slotStock);
                    }

                    detail.Details.Add(detailChild);

                }

                details.Add(detail);
            }

            return details;
        }
        private static readonly object lock_PayResultNotify = new object();
        public CustomJsonResult PayResultNotify(string operater, E_OrderPayPartner payPartner, E_OrderNotifyLogNotifyFrom from, string content)
        {
            LogUtil.Info("PayResultNotify");
            lock (lock_PayResultNotify)
            {

                Order order = null;
                string orderSn = "";
                string clientUserName = "";
                E_OrderPayWay orderPayWay = E_OrderPayWay.Unknow;
                bool isPaySuccess = false;
                if (payPartner == E_OrderPayPartner.Wechat)
                {

                    #region 解释微信支付协议
                    LogUtil.Info("解释微信支付协议");
                    orderPayWay = E_OrderPayWay.Wechat;

                    var dic = MyWeiXinSdk.CommonUtil.XmlToDictionary(content);
                    if (dic.ContainsKey("out_trade_no"))
                    {
                        orderSn = dic["out_trade_no"].ToString();
                    }

                    LogUtil.Info("解释微信支付协议，订单号：" + orderSn);


                    if (from == E_OrderNotifyLogNotifyFrom.OrderQuery)
                    {
                        if (dic.ContainsKey("out_trade_no") && dic.ContainsKey("trade_state"))
                        {
                            string trade_state = dic["trade_state"].ToString();
                            LogUtil.Info("解释微信支付协议，（trade_state）订单状态：" + trade_state);
                            if (trade_state == "SUCCESS")
                            {
                                isPaySuccess = true;
                            }
                        }
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        if (dic.ContainsKey("result_code"))
                        {
                            string result_code = dic["result_code"].ToString();
                            LogUtil.Info("解释微信支付协议，（result_code）订单状态：" + result_code);
                            if (result_code == "SUCCESS")
                            {
                                isPaySuccess = true;
                            }
                        }
                    }
                    #endregion
                }
                else if (payPartner == E_OrderPayPartner.AliPay)
                {
                    #region 解释支付宝支付协议
                    LogUtil.Info("解释支付宝支付协议");
                    orderPayWay = E_OrderPayWay.AliPay;
                    var dic = MyAlipaySdk.CommonUtil.FormStringToDictionary(content);

                    if (from == E_OrderNotifyLogNotifyFrom.OrderQuery)
                    {

                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        if (dic.ContainsKey("out_trade_no"))
                        {
                            orderSn = dic["out_trade_no"].ToString();
                        }

                        LogUtil.Info("解释支付宝支付协议，订单号：" + orderSn);

                        clientUserName = dic["buyer_logon_id"];

                        if (dic.ContainsKey("trade_status"))
                        {
                            string trade_status = dic["trade_status"].ToString();
                            LogUtil.Info("解释支付宝支付协议，（trade_status）订单状态：" + trade_status);
                            if (trade_status == "TRADE_SUCCESS")
                            {
                                isPaySuccess = true;
                            }
                        }

                    }

                    #endregion
                }

                else if (payPartner == E_OrderPayPartner.TongGuan)
                {
                    #region 解释 通莞支付协议

                    if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<AllQrcodePayAsynNotifyResult>(content);
                        if (result != null)
                        {
                            if (result.state == "0")
                            {
                                isPaySuccess = true;
                                orderSn = result.lowOrderId;

                                if (result.channelID == "WX")
                                {
                                    orderPayWay = E_OrderPayWay.Wechat;
                                }
                                if (result.channelID == "ZFB")
                                {
                                    orderPayWay = E_OrderPayWay.AliPay;
                                }

                            }
                        }
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.OrderQuery)
                    {
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<OrderQueryRequestResult>(content);
                        if (result != null)
                        {
                            if (result.status == "100")
                            {
                                if (result.state == "0")
                                {
                                    isPaySuccess = true;
                                    orderSn = result.lowOrderId;

                                    if (result.channelID == "WX")
                                    {
                                        orderPayWay = E_OrderPayWay.Wechat;
                                    }
                                    if (result.channelID == "ZFB")
                                    {
                                        orderPayWay = E_OrderPayWay.AliPay;
                                    }

                                }
                            }
                        }
                    }
                    #endregion
                }

                if (!string.IsNullOrEmpty(orderSn))
                {
                    if (isPaySuccess)
                    {
                        LogUtil.Info("解释支付协议结果，支付成功");

                        Dictionary<string, string> pms = new Dictionary<string, string>();
                        pms.Add("clientUserName", clientUserName);
                        PaySuccess(operater, orderSn, orderPayWay, DateTime.Now, pms);
                    }

                    order = CurrentDb.Order.Where(m => m.Sn == orderSn).FirstOrDefault();

                    if (order != null)
                    {
                        var mod_OrderNotifyLog = new OrderNotifyLog();
                        mod_OrderNotifyLog.Id = GuidUtil.New();
                        mod_OrderNotifyLog.MerchId = order.MerchId;
                        mod_OrderNotifyLog.OrderId = order.Id;
                        mod_OrderNotifyLog.OrderSn = order.Sn;
                        mod_OrderNotifyLog.NotifyContent = content;
                        mod_OrderNotifyLog.NotifyFrom = from;
                        mod_OrderNotifyLog.NotifyType = E_OrderNotifyLogNotifyType.Pay;
                        mod_OrderNotifyLog.CreateTime = DateTime.Now;
                        mod_OrderNotifyLog.Creator = operater;
                        CurrentDb.OrderNotifyLog.Add(mod_OrderNotifyLog);
                        CurrentDb.SaveChanges();
                    }
                }
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
        public CustomJsonResult PaySuccess(string operater, string orderSn, E_OrderPayWay payWay, DateTime completedTime, Dictionary<string, string> pms = null)
        {
            CustomJsonResult result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                LogUtil.Info("orderSn:" + orderSn);

                var order = CurrentDb.Order.Where(m => m.Sn == orderSn).FirstOrDefault();

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", orderSn));
                }

                if (order.Status == E_OrderStatus.Payed || order.Status == E_OrderStatus.Completed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("订单号({0})已经支付通知成功", orderSn));
                }

                if (order.Status != E_OrderStatus.WaitPay)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", orderSn));
                }

                LogUtil.Info("orderSn2:" + orderSn);

                order.PayWay = payWay;
                order.Status = E_OrderStatus.Payed;
                order.PayTime = DateTime.Now;
                order.MendTime = DateTime.Now;
                order.Mender = operater;

                if (pms != null)
                {
                    if (pms.ContainsKey("clientUserName"))
                    {
                        order.ClientUserName = pms["clientUserName"];
                    }
                }

                var rptOrder = new RptOrder();
                rptOrder.Id = GuidUtil.New();
                rptOrder.OrderId = order.Id;
                rptOrder.OrderSn = order.Sn;
                rptOrder.MerchId = order.MerchId;
                rptOrder.StoreId = order.StoreId;
                rptOrder.StoreName = order.StoreName;
                rptOrder.ClientUserId = order.ClientUserId;
                rptOrder.TradeType = E_RptOrderTradeType.Pay;
                rptOrder.TradeAmount = order.ChargeAmount;
                rptOrder.Quantity = order.Quantity;
                rptOrder.TradeTime = order.PayTime.Value;
                CurrentDb.RptOrder.Add(rptOrder);


                var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).ToList();

                foreach (var orderDetail in orderDetails)
                {
                    orderDetail.Status = E_OrderStatus.Payed;
                    orderDetail.Mender = GuidUtil.Empty();
                    orderDetail.MendTime = DateTime.Now;

                    var rptOrderDetails = new RptOrderDetails();
                    rptOrderDetails.Id = GuidUtil.New();
                    rptOrderDetails.RptOrderId = rptOrder.Id;
                    rptOrderDetails.OrderId = order.Id;
                    rptOrderDetails.MerchId = order.MerchId;
                    rptOrderDetails.ClientUserId = order.ClientUserId;
                    rptOrderDetails.StoreId = order.StoreId;
                    rptOrderDetails.StoreName = order.StoreName;
                    rptOrderDetails.Quantity = orderDetail.Quantity;
                    rptOrderDetails.TradeType = E_RptOrderTradeType.Pay;
                    rptOrderDetails.TradeTime = order.PayTime.Value;
                    rptOrderDetails.TradeAmount = orderDetail.ChargeAmount;
                    rptOrderDetails.SellChannelRefId = orderDetail.SellChannelRefId;
                    rptOrderDetails.SellChannelRefName = orderDetail.SellChannelRefName;
                    rptOrderDetails.SellChannelRefType = orderDetail.SellChannelRefType;
                    CurrentDb.RptOrderDetails.Add(rptOrderDetails);

                    var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderDetailsId == orderDetail.Id).ToList();

                    foreach (var orderDetailsChild in orderDetailsChilds)
                    {
                        orderDetailsChild.Status = E_OrderStatus.Payed;
                        orderDetailsChild.Mender = GuidUtil.Empty();
                        orderDetailsChild.MendTime = DateTime.Now;

                        var rptOrderDetailsChild = new RptOrderDetailsChild();
                        rptOrderDetailsChild.Id = GuidUtil.New();
                        rptOrderDetailsChild.RptOrderId = rptOrder.Id;
                        rptOrderDetailsChild.RptOrderDetailsId = rptOrderDetails.Id;
                        rptOrderDetailsChild.OrderId = order.Id;
                        rptOrderDetailsChild.MerchId = order.MerchId;
                        rptOrderDetailsChild.ClientUserId = order.ClientUserId;
                        rptOrderDetailsChild.StoreId = order.StoreId;
                        rptOrderDetailsChild.StoreName = order.StoreName;
                        rptOrderDetailsChild.Quantity = orderDetailsChild.Quantity;
                        rptOrderDetailsChild.PrdProductId = orderDetailsChild.PrdProductId;
                        rptOrderDetailsChild.PrdProductSkuId = orderDetailsChild.PrdProductSkuId;
                        rptOrderDetailsChild.PrdProductSkuName = orderDetailsChild.PrdProductSkuName;
                        rptOrderDetailsChild.TradeType = E_RptOrderTradeType.Pay;
                        rptOrderDetailsChild.TradeTime = order.PayTime.Value;
                        rptOrderDetailsChild.TradeAmount = orderDetailsChild.ChargeAmount;
                        rptOrderDetailsChild.SellChannelRefId = orderDetailsChild.SellChannelRefId;
                        rptOrderDetailsChild.SellChannelRefName = orderDetailsChild.SellChannelRefName;
                        rptOrderDetailsChild.SellChannelRefType = orderDetailsChild.SellChannelRefType;
                        CurrentDb.RptOrderDetailsChild.Add(rptOrderDetailsChild);
                    }

                }


                var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == order.Id).ToList();

                foreach (var item in orderDetailsChildSons)
                {
                    item.Status = E_OrderDetailsChildSonStatus.Payed;
                    item.Mender = GuidUtil.Empty();
                    item.MendTime = DateTime.Now;
                }

                var childSons = (
                    from q in orderDetailsChildSons
                    group q by new { q.PrdProductSkuId, q.Quantity, q.SellChannelRefType, q.SlotId, q.SellChannelRefId } into b
                    select new { b.Key.PrdProductSkuId, b.Key.SellChannelRefId, b.Key.SellChannelRefType, b.Key.SlotId, Quantity = b.Sum(c => c.Quantity) }).ToList();




                foreach (var item in childSons)
                {
                    BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderPaySuccess, order.MerchId, order.StoreId, item.SellChannelRefId, item.SlotId, item.PrdProductSkuId, item.Quantity);

                }

                CurrentDb.SaveChanges();
                ts.Complete();




                Task4Factory.Global.Exit(order.Id);
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, string.Format("支付完成通知：订单号({0})通知成功", orderSn));
            }

            return result;
        }
        public CustomJsonResult<RetPayResultQuery> PayResultQuery(string operater, string orderId)
        {
            var result = new CustomJsonResult<RetPayResultQuery>();

            var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

            if (order == null)
            {
                return new CustomJsonResult<RetPayResultQuery>(ResultType.Failure, ResultCode.Failure, "找不到订单", null);
            }

            var ret = new RetPayResultQuery();

            ret.OrderId = order.Id;
            ret.OrderSn = order.Sn;
            ret.Status = order.Status;

            result = new CustomJsonResult<RetPayResultQuery>(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
        public CustomJsonResult Cancle(string operater, string orderId, string cancelReason)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Id == orderId).FirstOrDefault();

                if (order == null)
                {
                    LogUtil.Info(string.Format("该订单号:{0},找不到", orderId));
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("该订单号:{0},找不到", orderId));
                }

                if (order.Status == E_OrderStatus.Cancled)
                {
                    return new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单已经取消");
                }

                if (order.Status == E_OrderStatus.Payed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经支付成功");
                }

                if (order.Status == E_OrderStatus.Completed)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单已经完成");
                }

                if (order.Status != E_OrderStatus.Payed && order.Status != E_OrderStatus.Completed)
                {
                    order.Status = E_OrderStatus.Cancled;
                    order.Mender = GuidUtil.Empty();
                    order.MendTime = DateTime.Now;
                    order.CancledTime = DateTime.Now;
                    order.CancelReason = cancelReason;

                    var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderDetails)
                    {
                        item.Status = E_OrderStatus.Cancled;
                        item.Mender = GuidUtil.Empty();
                        item.MendTime = DateTime.Now;
                    }


                    var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderDetailsChilds)
                    {
                        item.Status = E_OrderStatus.Cancled;
                        item.Mender = GuidUtil.Empty();
                        item.MendTime = DateTime.Now;
                    }

                    var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderDetailsChildSons)
                    {
                        item.Status = E_OrderDetailsChildSonStatus.Cancled;
                        item.Mender = GuidUtil.Empty();
                        item.MendTime = DateTime.Now;
                    }

                    var childSons = (
                        from q in orderDetailsChildSons
                        group q by new { q.PrdProductSkuId, q.Quantity, q.SellChannelRefType, q.SlotId, q.SellChannelRefId } into b
                        select new { b.Key.PrdProductSkuId, b.Key.SellChannelRefId, b.Key.SellChannelRefType, b.Key.SlotId, Quantity = b.Sum(c => c.Quantity) }).ToList();


                    foreach (var item in childSons)
                    {
                        BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderCancle, order.MerchId, order.StoreId, item.SellChannelRefId, item.SlotId, item.PrdProductSkuId, item.Quantity);
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    Task4Factory.Global.Exit(order.Id);

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "已取消");
                }
            }

            return result;
        }
        public CustomJsonResult BuildPayParams(string operater, RopOrderBuildPayParams rop)
        {

            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                var order = CurrentDb.Order.Where(m => m.Id == rop.OrderId).FirstOrDefault();

                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该订单数据");
                }

                order.PayExpireTime = DateTime.Now.AddMinutes(5);
                order.PayCaller = rop.PayCaller;


                var orderAttach = new BLL.Biz.OrderAttachModel();

                switch (rop.PayCaller)
                {
                    case E_OrderPayCaller.AlipayByNative:
                        #region AlipayByNative
                        order.PayPartner = E_OrderPayPartner.AliPay;
                        order.PayWay = E_OrderPayWay.AliPay;
                        var alipayByNative_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetAlipayMpAppInfoConfig(order.MerchId);
                        var alipayByNative_UnifiedOrder = SdkFactory.Alipay.UnifiedOrderByNative(alipayByNative_AppInfoConfig, order.MerchId, order.StoreId, order.Sn, 0.01m, "", CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);
                        if (string.IsNullOrEmpty(alipayByNative_UnifiedOrder.CodeUrl))
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                        }

                        order.PayQrCodeUrl = alipayByNative_UnifiedOrder.CodeUrl;

                        var alipayByNative_PayParams = new { PayUrl = order.PayQrCodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", alipayByNative_PayParams);
                        #endregion 
                        break;
                    case E_OrderPayCaller.WechatByNative:
                        #region WechatByNative
                        order.PayPartner = E_OrderPayPartner.Wechat;
                        order.PayWay = E_OrderPayWay.Wechat;
                        var wechatByNative_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);
                        var wechatByNative_UnifiedOrder = SdkFactory.Wx.UnifiedOrderByNative(wechatByNative_AppInfoConfig, order.MerchId, order.StoreId, order.Sn, 0.01m, "", CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);
                        if (string.IsNullOrEmpty(wechatByNative_UnifiedOrder.PrepayId))
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                        }

                        order.PayPrepayId = wechatByNative_UnifiedOrder.PrepayId;
                        order.PayQrCodeUrl = wechatByNative_UnifiedOrder.CodeUrl;

                        var wechatByNative_PayParams = new { PayUrl = order.PayQrCodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wechatByNative_PayParams);
                        #endregion
                        break;
                    case E_OrderPayCaller.WechatByMp:
                        #region WechatByMp
                        order.PayPartner = E_OrderPayPartner.Wechat;
                        order.PayWay = E_OrderPayWay.Wechat;
                        var wechatByMp_UserInfo = CurrentDb.WxUserInfo.Where(m => m.ClientUserId == order.ClientUserId).FirstOrDefault();

                        if (wechatByMp_UserInfo == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到该用户数据");
                        }

                        var wechatByMp_AppInfoConfig = BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);

                        if (wechatByMp_AppInfoConfig == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "商户信息认证失败");
                        }

                        orderAttach.MerchId = order.MerchId;
                        orderAttach.StoreId = order.StoreId;
                        orderAttach.PayCaller = rop.PayCaller;

                        var wechatByMp_UnifiedOrder = SdkFactory.Wx.UnifiedOrderByJsApi(wechatByMp_AppInfoConfig, wechatByMp_UserInfo.OpenId, order.Sn, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);

                        if (string.IsNullOrEmpty(wechatByMp_UnifiedOrder.PrepayId))
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                        }

                        order.PayPrepayId = wechatByMp_UnifiedOrder.PrepayId;

                        var pms = SdkFactory.Wx.GetJsApiPayParams(wechatByMp_AppInfoConfig, order.Id, order.Sn, wechatByMp_UnifiedOrder.PrepayId);

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", pms);
                        #endregion 
                        break;
                    case E_OrderPayCaller.TongGuanByAllQrcodePay:
                        #region TongGuanByAllQrcodePay
                        order.PayPartner = E_OrderPayPartner.TongGuan;
                        var tongGuanPay_PayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetTongGuanPayInfoConfg(order.MerchId);
                        var tongGuanPay_AllQrcodePay = SdkFactory.TongGuan.AllQrcodePay(tongGuanPay_PayInfoConfig, order.MerchId, order.StoreId, order.Sn, 0.01m, "", CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);
                        if (string.IsNullOrEmpty(tongGuanPay_AllQrcodePay.codeUrl))
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                        }

                        order.PayQrCodeUrl = tongGuanPay_AllQrcodePay.codeUrl;

                        var tongGuanPay_AllQrcodePay_PayParams = new { PayUrl = order.PayQrCodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", tongGuanPay_AllQrcodePay_PayParams);

                        #endregion
                        break;
                    default:
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                }

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }
    }
}
