﻿using LocalS.BLL;
using LocalS.Entity;
using LocalS.Mq;
using LocalS.Mq.MqByRedis;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class OrderService : BaseDbContext
    {
        public string BuildPickCode()
        {
            // todo
            string pickCode = "";
            Random rd = new Random();
            int num = rd.Next(100000, 1000000);
            pickCode = num.ToString();

            return pickCode;
        }
        public CustomJsonResult<RetOrderReserve> Reserve(string operater, RopOrderReserve rop)
        {
            CustomJsonResult<RetOrderReserve> result = new CustomJsonResult<RetOrderReserve>();

            if (rop.ProductSkus == null || rop.ProductSkus.Count == 0)
            {
                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定商品为空", null);
            }

            if (rop.ReserveMode == E_ReserveMode.Unknow)
            {
                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定方式不支持", null);
            }

            if (rop.ReserveMode == E_ReserveMode.OffLine)
            {
                if (string.IsNullOrEmpty(rop.SellChannelRefId))
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定机器未选择", null);
                }
            }

            var store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();
            if (store == null)
            {
                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定店铺无效", null);
            }

            using (TransactionScope ts = new TransactionScope())
            {
                RetOrderReserve ret = new RetOrderReserve();

                //检查是否有可买的商品
                List<string> warn_tips = new List<string>();

                List<PrdProductSkuModel> productSkus = new List<BLL.PrdProductSkuModel>();

                foreach (var productSku in rop.ProductSkus)
                {
                    var l_ProductSku = BizFactory.PrdProduct.GetProductSkuInfo(productSku.Id);
                    if (l_ProductSku == null)
                    {
                        warn_tips.Add(string.Format("{0}商品库存信息不存在", l_ProductSku.Name));
                    }
                    else
                    {
                        var sellQuantity = 0;

                        if (rop.ReserveMode == E_ReserveMode.OffLine)
                        {
                            sellQuantity = l_ProductSku.Stocks.Where(m => m.RefType == E_SellChannelRefType.Machine && m.RefId == rop.SellChannelRefId).Sum(m => m.SellQuantity);
                        }
                        else if (rop.ReserveMode == E_ReserveMode.Online)
                        {
                            if (productSku.ReceptionMode == E_ReceptionMode.Machine)
                            {
                                sellQuantity = l_ProductSku.Stocks.Where(m => m.RefType == E_SellChannelRefType.Machine).Sum(m => m.SellQuantity);
                            }
                            else if (productSku.ReceptionMode == E_ReceptionMode.Express)
                            {
                                sellQuantity = l_ProductSku.Stocks.Where(m => m.RefType == E_SellChannelRefType.Express).Sum(m => m.SellQuantity);
                            }
                            else if (productSku.ReceptionMode == E_ReceptionMode.SelfTake)
                            {
                                sellQuantity = l_ProductSku.Stocks.Where(m => m.RefType == E_SellChannelRefType.SelfTake).Sum(m => m.SellQuantity);
                            }
                        }

                        if (l_ProductSku.IsOffSell)
                        {
                            warn_tips.Add(string.Format("{0}已经下架", l_ProductSku.Name));
                        }
                        else
                        {
                            if (sellQuantity < productSku.Quantity)
                            {
                                warn_tips.Add(string.Format("{0}的可销售数量为{1}个", l_ProductSku.Name, sellQuantity));
                            }
                        }

                        productSkus.Add(l_ProductSku);
                    }
                }

                if (warn_tips.Count > 0)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, string.Join(";", warn_tips.ToArray()), null);
                }

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
                order.PayWay = rop.PayWay;
                order.PayCaller = rop.PayCaller;
                order.PickCode = BuildPickCode();
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

                var reserveDetails = GetReserveDetail(rop.ProductSkus, productSkus);

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
                        case E_SellChannelRefType.Express:
                            orderDetails.SellChannelRefName = "【快递】";
                            orderDetails.Receiver = rop.Receiver;
                            orderDetails.ReceiverPhone = rop.ReceiverPhone;
                            orderDetails.ReceptionAddress = rop.ReceptionAddress;
                            orderDetails.SellChannelRefType = E_SellChannelRefType.Express;
                            orderDetails.SellChannelRefId = GuidUtil.Empty();
                            break;
                        case E_SellChannelRefType.SelfTake:
                            orderDetails.SellChannelRefName = "【店内自取】";
                            orderDetails.Receiver = rop.Receiver;
                            orderDetails.ReceiverPhone = rop.ReceiverPhone;
                            orderDetails.ReceptionAddress = rop.ReceptionAddress;
                            orderDetails.SellChannelRefType = E_SellChannelRefType.SelfTake;
                            orderDetails.SellChannelRefId = GuidUtil.Empty();
                            break;
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
                        orderDetailsChild.PrdProductSkuId = detailsChild.PrdProductSkuId;
                        orderDetailsChild.PrdProductId = detailsChild.PrdProductId;
                        orderDetailsChild.PrdProductSkuName = detailsChild.PrdProductSkuName;
                        orderDetailsChild.PrdProductSkuMainImgUrl = detailsChild.PrdProductSkuMainImgUrl;
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
                            orderDetailsChildSon.PrdProductSkuId = detailsChildSon.PrdProductSkuId;
                            orderDetailsChildSon.PrdProductId = orderDetailsChild.PrdProductId;
                            orderDetailsChildSon.PrdProductSkuName = detailsChildSon.PrdProductSkuName;
                            orderDetailsChildSon.PrdProductSkuMainImgUrl = detailsChildSon.PrdProductSkuMainImgUrl;
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
                            var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.PrdProductSkuId == slotStock.PrdProductSkuId && m.SlotId == slotStock.SlotId && m.RefId == slotStock.SellChannelRefId).FirstOrDefault();

                            sellChannelStock.LockQuantity += slotStock.Quantity;
                            sellChannelStock.SellQuantity -= slotStock.Quantity;
                            sellChannelStock.Mender = operater;
                            sellChannelStock.MendTime = DateTime.Now;

                            var sellChannelStockLog = new SellChannelStockLog();
                            sellChannelStockLog.Id = GuidUtil.New();
                            sellChannelStockLog.MerchId = store.MerchId;
                            sellChannelStockLog.RefType = slotStock.SellChannelRefType;
                            sellChannelStockLog.RefId = slotStock.SellChannelRefId;
                            sellChannelStockLog.SlotId = slotStock.SlotId;
                            sellChannelStockLog.PrdProductSkuId = slotStock.PrdProductSkuId;
                            sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                            sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                            sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                            sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.Lock;
                            sellChannelStockLog.ChangeQuantity = slotStock.Quantity;
                            sellChannelStockLog.Creator = operater;
                            sellChannelStockLog.CreateTime = DateTime.Now;
                            sellChannelStockLog.RemarkByDev = string.Format("预定锁定库存：{0}", slotStock.Quantity);
                            CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);


                        }
                    }
                }


                //生产支付信息

                var orderAttach = new LocalS.BLL.Biz.OrderAttachModel();
                orderAttach.MerchId = order.MerchId;
                orderAttach.StoreId = order.StoreId;
                orderAttach.PayCaller = rop.PayCaller;

                ret.OrderId = order.Id;
                ret.OrderSn = order.Sn;

                switch (rop.PayCaller)
                {
                    case E_OrderPayCaller.WechatByMp:

                        break;
                    case E_OrderPayCaller.WechatByNative:
                        var wxPaAppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxPaAppInfoConfig(order.MerchId);
                        var ret_UnifiedOrder = Lumos.BLL.SdkFactory.Wx.UnifiedOrderByNative(wxPaAppInfoConfig, order.MerchId, order.Sn, 0.01m, "", CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);

                        if (string.IsNullOrEmpty(ret_UnifiedOrder.PrepayId))
                        {
                            return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败", null);
                        }

                        order.PayPrepayId = ret_UnifiedOrder.PrepayId;
                        order.PayQrCodeUrl = ret_UnifiedOrder.CodeUrl;

                        ret.PayUrl = order.PayQrCodeUrl;
                        ret.ChargeAmount = order.ChargeAmount.ToF2Price();

                        break;
                }

                CurrentDb.Order.Add(order);
                CurrentDb.SaveChanges();
                ts.Complete();

                ReidsMqFactory.Global.PushOrderReserve(new Mq.MqMessageConentModel.OrderReserveModel { OrderId = order.Id });

                result = new CustomJsonResult<RetOrderReserve>(ResultType.Success, ResultCode.Success, "预定成功", ret);

            }

            return result;

        }

        private List<OrderReserveDetail> GetReserveDetail(List<RopOrderReserve.ProductSku> reserveDetails, List<PrdProductSkuModel> productSkus)
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
                        case E_ReceptionMode.Express:
                            channelType = E_SellChannelRefType.Express;
                            break;
                        case E_ReceptionMode.SelfTake:
                            channelType = E_SellChannelRefType.SelfTake;
                            break;
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
                            int reservedQuantity = detailChildSons.Where(m => m.PrdProductSkuId == reserveDetail.Id && m.SellChannelRefType == channelType).Sum(m => m.Quantity);//已订的数量
                            int needReserveQuantity = reserveDetail.Quantity;//需要订的数量
                            if (reservedQuantity != needReserveQuantity)
                            {
                                var detailChildSon = new OrderReserveDetail.DetailChildSon();
                                detailChildSon.Id = GuidUtil.New();
                                detailChildSon.SellChannelRefType = item.RefType;
                                detailChildSon.SellChannelRefId = item.RefId;
                                detailChildSon.ReceptionMode = receptionMode;
                                detailChildSon.PrdProductSkuId = productSku.Id;
                                detailChildSon.PrdProductId = productSku.PrdProductId;
                                detailChildSon.PrdProductSkuName = productSku.Name;
                                detailChildSon.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                                detailChildSon.SlotId = item.SlotId;
                                detailChildSon.Quantity = 1;
                                detailChildSon.SalePrice = productSku.SalePrice;
                                detailChildSon.SalePriceByVip = productSku.SalePriceByVip;
                                detailChildSon.OriginalAmount = detailChildSon.Quantity * productSku.SalePrice;
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
                                             c.PrdProductSkuId
                                         }).Distinct().ToList();

                foreach (var detailChildGroup in detailChildGroups)
                {

                    var detailChild = new OrderReserveDetail.DetailChild();
                    detailChild.SellChannelRefType = detailChildGroup.SellChannelRefType;
                    detailChild.SellChannelRefId = detailChildGroup.SellChannelRefId;
                    detailChild.PrdProductSkuId = detailChildGroup.PrdProductSkuId;
                    detailChild.PrdProductId = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).First().PrdProductId;
                    detailChild.PrdProductSkuName = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).First().PrdProductSkuName;
                    detailChild.PrdProductSkuMainImgUrl = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).First().PrdProductSkuMainImgUrl;
                    detailChild.SalePrice = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).First().SalePrice;
                    detailChild.SalePriceByVip = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).First().SalePriceByVip;
                    detailChild.Quantity = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).Sum(m => m.Quantity);
                    detailChild.OriginalAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).Sum(m => m.OriginalAmount);
                    detailChild.DiscountAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).Sum(m => m.DiscountAmount);
                    detailChild.ChargeAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.PrdProductSkuId == detailChildGroup.PrdProductSkuId).Sum(m => m.ChargeAmount);

                    var detailChildSonGroups = (from c in detailChildSons
                                                where c.SellChannelRefId == detailChildGroup.SellChannelRefId
                                             && c.PrdProductSkuId == detailChildGroup.PrdProductSkuId
                                                select new
                                                {
                                                    c.Id,
                                                    c.ReceptionMode,
                                                    c.SellChannelRefType,
                                                    c.SellChannelRefId,
                                                    c.PrdProductSkuId,
                                                    c.SlotId,
                                                    c.Quantity,
                                                    c.SalePrice,
                                                    c.SalePriceByVip,
                                                    c.PrdProductSkuMainImgUrl,
                                                    c.PrdProductSkuName,
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
                        detailChildSon.PrdProductSkuId = detailChildSonGroup.PrdProductSkuId;
                        detailChildSon.SlotId = detailChildSonGroup.SlotId;
                        detailChildSon.Quantity = detailChildSonGroup.Quantity;
                        detailChildSon.PrdProductSkuName = detailChildSonGroup.PrdProductSkuName;
                        detailChildSon.PrdProductSkuMainImgUrl = detailChildSonGroup.PrdProductSkuMainImgUrl;
                        detailChildSon.SalePrice = detailChildSonGroup.SalePrice;
                        detailChildSon.SalePriceByVip = detailChildSonGroup.SalePriceByVip;
                        detailChildSon.OriginalAmount = detailChildSonGroup.OriginalAmount;
                        detailChildSon.DiscountAmount = detailChildSonGroup.DiscountAmount;
                        detailChildSon.ChargeAmount = detailChildSonGroup.ChargeAmount;
                        detailChild.Details.Add(detailChildSon);
                    }



                    var slotStockGroups = (from c in detailChildSons
                                           where c.SellChannelRefId == detailChildGroup.SellChannelRefId
                                        && c.PrdProductSkuId == detailChildGroup.PrdProductSkuId
                                           select new
                                           {
                                               c.SellChannelRefType,
                                               c.SellChannelRefId,
                                               c.PrdProductSkuId,
                                               c.SlotId
                                           }).Distinct().ToList();


                    foreach (var slotStockGroup in slotStockGroups)
                    {
                        var slotStock = new OrderReserveDetail.SlotStock();
                        slotStock.SellChannelRefType = slotStockGroup.SellChannelRefType;
                        slotStock.SellChannelRefId = slotStockGroup.SellChannelRefId;
                        slotStock.PrdProductSkuId = slotStockGroup.PrdProductSkuId;
                        slotStock.SlotId = slotStockGroup.SlotId;
                        slotStock.Quantity = detailChildSons.Where(m => m.SellChannelRefType == slotStockGroup.SellChannelRefType && m.SellChannelRefId == slotStockGroup.SellChannelRefId && m.PrdProductSkuId == slotStockGroup.PrdProductSkuId && m.SlotId == slotStockGroup.SlotId).Sum(m => m.Quantity);
                        detailChild.SlotStock.Add(slotStock);
                    }

                    detail.Details.Add(detailChild);

                }

                details.Add(detail);
            }

            return details;
        }

        private static readonly object lock_PayResultNotify = new object();
        public CustomJsonResult PayResultNotify(string operater, E_OrderNotifyLogNotifyFrom from, string content, string orderSn, out bool isPaySuccessed)
        {
            lock (lock_PayResultNotify)
            {
                bool m_isPaySuccessed = false;
                var mod_OrderNotifyLog = new OrderNotifyLog();

                switch (from)
                {
                    case E_OrderNotifyLogNotifyFrom.WebApp:
                        if (content == "chooseWXPay:ok")
                        {
                            // PayCompleted(operater, orderSn, this.DateTime);
                        }
                        break;
                    case E_OrderNotifyLogNotifyFrom.OrderQuery:
                        var dic1 = WeiXinSdk.CommonUtil.ToDictionary(content);
                        if (dic1.ContainsKey("out_trade_no") && dic1.ContainsKey("trade_state"))
                        {
                            orderSn = dic1["out_trade_no"].ToString();
                            string trade_state = dic1["trade_state"].ToString();
                            if (trade_state == "SUCCESS")
                            {
                                m_isPaySuccessed = true;
                                PayCompleted(operater, orderSn, DateTime.Now);
                            }
                        }
                        break;
                    case E_OrderNotifyLogNotifyFrom.NotifyUrl:
                        var dic2 = WeiXinSdk.CommonUtil.ToDictionary(content);
                        if (dic2.ContainsKey("out_trade_no") && dic2.ContainsKey("result_code"))
                        {
                            orderSn = dic2["out_trade_no"].ToString();
                            string result_code = dic2["result_code"].ToString();

                            if (result_code == "SUCCESS")
                            {
                                m_isPaySuccessed = true;
                                PayCompleted(operater, orderSn, DateTime.Now);
                            }
                        }
                        break;
                }

                var order = CurrentDb.Order.Where(m => m.Sn == orderSn).FirstOrDefault();
                if (order != null)
                {
                    mod_OrderNotifyLog.MerchId = order.MerchId;
                    mod_OrderNotifyLog.OrderId = order.Id;
                    mod_OrderNotifyLog.OrderSn = order.Sn;
                }
                mod_OrderNotifyLog.Id = GuidUtil.New();
                mod_OrderNotifyLog.NotifyContent = content;
                mod_OrderNotifyLog.NotifyFrom = from;
                mod_OrderNotifyLog.NotifyType = E_OrderNotifyLogNotifyType.Pay;
                mod_OrderNotifyLog.CreateTime = DateTime.Now;
                mod_OrderNotifyLog.Creator = operater;
                CurrentDb.OrderNotifyLog.Add(mod_OrderNotifyLog);
                CurrentDb.SaveChanges();

                isPaySuccessed = m_isPaySuccessed;

                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            }
        }

        public CustomJsonResult PayCompleted(string operater, string orderSn, DateTime completedTime)
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

                order.Status = E_OrderStatus.Payed;
                order.PayTime = DateTime.Now;
                order.MendTime = DateTime.Now;
                order.Mender = operater;

                var orderDetails = CurrentDb.OrderDetails.Where(m => m.OrderId == order.Id).ToList();

                foreach (var item in orderDetails)
                {
                    item.Status = E_OrderStatus.Payed;
                    item.Mender = GuidUtil.Empty();
                    item.MendTime = DateTime.Now;
                }


                var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == order.Id).ToList();

                foreach (var item in orderDetailsChilds)
                {
                    item.Status = E_OrderStatus.Payed;
                    item.Mender = GuidUtil.Empty();
                    item.MendTime = DateTime.Now;
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

                foreach (var childSon in childSons)
                {
                    var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == order.MerchId && m.PrdProductSkuId == childSon.PrdProductSkuId && m.SlotId == childSon.SlotId && m.RefId == childSon.SellChannelRefId && m.RefType == childSon.SellChannelRefType).FirstOrDefault();

                    sellChannelStock.LockQuantity -= childSon.Quantity;
                    sellChannelStock.SumQuantity -= childSon.Quantity;
                    sellChannelStock.Mender = operater;
                    sellChannelStock.MendTime = DateTime.Now;

                    var sellChannelStockLog = new SellChannelStockLog();
                    sellChannelStockLog.Id = GuidUtil.New();
                    sellChannelStockLog.MerchId = order.MerchId;
                    sellChannelStockLog.RefId = childSon.SellChannelRefId;
                    sellChannelStockLog.RefType = childSon.SellChannelRefType;
                    sellChannelStockLog.SlotId = childSon.SlotId;
                    sellChannelStockLog.PrdProductSkuId = childSon.PrdProductSkuId;
                    sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                    sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                    sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                    sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.Sales;
                    sellChannelStockLog.ChangeQuantity = childSon.Quantity;
                    sellChannelStockLog.Creator = operater;
                    sellChannelStockLog.CreateTime = DateTime.Now;
                    sellChannelStockLog.RemarkByDev = string.Format("取消订单，恢复库存：{0}", childSon.Quantity);
                    CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                }


                CurrentDb.SaveChanges();
                ts.Complete();

                ReidsMqFactory.Global.PushOrderPayCompleted(new Mq.MqMessageConentModel.OrderPayCompletedModel { OrderId = order.Id });

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

                    foreach (var childSon in childSons)
                    {
                        var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == order.MerchId && m.PrdProductSkuId == childSon.PrdProductSkuId && m.SlotId == childSon.SlotId && m.RefId == childSon.SellChannelRefId && m.RefType == childSon.SellChannelRefType).FirstOrDefault();

                        sellChannelStock.LockQuantity -= childSon.Quantity;
                        sellChannelStock.SellQuantity += childSon.Quantity;
                        sellChannelStock.Mender = operater;
                        sellChannelStock.MendTime = DateTime.Now;

                        var sellChannelStockLog = new SellChannelStockLog();
                        sellChannelStockLog.Id = GuidUtil.New();
                        sellChannelStockLog.MerchId = order.MerchId;
                        sellChannelStockLog.RefId = childSon.SellChannelRefId;
                        sellChannelStockLog.RefType = childSon.SellChannelRefType;
                        sellChannelStockLog.SlotId = childSon.SlotId;
                        sellChannelStockLog.PrdProductSkuId = childSon.PrdProductSkuId;
                        sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                        sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                        sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                        sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.Lock;
                        sellChannelStockLog.ChangeQuantity = childSon.Quantity;
                        sellChannelStockLog.Creator = operater;
                        sellChannelStockLog.CreateTime = DateTime.Now;
                        sellChannelStockLog.RemarkByDev = string.Format("取消订单，恢复库存：{0}", childSon.Quantity);
                        CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    ReidsMqFactory.Global.PushOrderCancle(new Mq.MqMessageConentModel.OrderCancleModel { OrderId = order.Id });

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "已取消");
                }
            }

            return result;
        }
    }
}
