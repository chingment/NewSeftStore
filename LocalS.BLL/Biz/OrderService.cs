using LocalS.BLL;
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
using TgPaySdk;
using MyAlipaySdk;
using System.Configuration;

namespace LocalS.BLL.Biz
{
    public class OrderService : BaseDbContext
    {
        public Order GetOne(string id)
        {
            var order = CurrentDb.Order.Where(m => m.Id == id).FirstOrDefault();

            return order;
        }

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
                    order.PickupCode = RedisSnUtil.BuildPickupCode();

                    if (order.PickupCode == null)
                    {
                        return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "预定下单生成取货码失败", null);
                    }

                    order.SubmittedTime = DateTime.Now;
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
                        orderDetails.Quantity = detail.Quantity;
                        orderDetails.Creator = operater;
                        orderDetails.CreateTime = DateTime.Now;
                        CurrentDb.OrderDetails.Add(orderDetails);

                        foreach (var detailsChild in detail.Details)
                        {
                            var productSku = bizProductSkus.Where(m => m.Id == detailsChild.ProductSkuId).FirstOrDefault();

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
                            orderDetailsChild.PrdProductSkuName = productSku.Name;
                            orderDetailsChild.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                            orderDetailsChild.PrdProductSkuSpecDes = productSku.SpecDes;
                            orderDetailsChild.PrdProductSkuProducer = productSku.Producer;
                            orderDetailsChild.PrdProductSkuBarCode = productSku.BarCode;
                            orderDetailsChild.PrdProductSkuCumCode = productSku.CumCode;
                            orderDetailsChild.SalePrice = detailsChild.SalePrice;
                            orderDetailsChild.SalePriceByVip = detailsChild.SalePriceByVip;
                            orderDetailsChild.Quantity = detailsChild.Quantity;
                            orderDetailsChild.OriginalAmount = detailsChild.OriginalAmount;
                            orderDetailsChild.DiscountAmount = detailsChild.DiscountAmount;
                            orderDetailsChild.ChargeAmount = detailsChild.ChargeAmount;
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
                                orderDetailsChildSon.StoreName = order.StoreName;
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
                                orderDetailsChildSon.PrdProductSkuName = productSku.Name;
                                orderDetailsChildSon.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                                orderDetailsChildSon.PrdProductSkuSpecDes = productSku.SpecDes;
                                orderDetailsChildSon.PrdProductSkuProducer = productSku.Producer;
                                orderDetailsChildSon.PrdProductSkuBarCode = productSku.BarCode;
                                orderDetailsChildSon.PrdProductSkuCumCode = productSku.CumCode;
                                orderDetailsChildSon.SalePrice = detailsChildSon.SalePrice;
                                orderDetailsChildSon.SalePriceByVip = detailsChildSon.SalePriceByVip;
                                orderDetailsChildSon.Quantity = detailsChildSon.Quantity;
                                orderDetailsChildSon.OriginalAmount = detailsChildSon.OriginalAmount;
                                orderDetailsChildSon.DiscountAmount = detailsChildSon.DiscountAmount;
                                orderDetailsChildSon.ChargeAmount = detailsChildSon.ChargeAmount;
                                orderDetailsChildSon.Creator = operater;
                                orderDetailsChildSon.CreateTime = DateTime.Now;
                                orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.WaitPay;
                                CurrentDb.OrderDetailsChildSon.Add(orderDetailsChildSon);
                            }

                            foreach (var slotStock in detailsChild.SlotStock)
                            {
                                BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderReserveSuccess, order.MerchId, order.StoreId, slotStock.SellChannelRefId, slotStock.SlotId, slotStock.ProductSkuId, slotStock.Quantity);
                            }
                        }
                    }

                    CurrentDb.Order.Add(order);
                    CurrentDb.SaveChanges();
                    ts.Complete();


                    Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPay, order.Id, order.PayExpireTime.Value, order);

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
                string orderSn = "";
                string payPartnerOrderSn = "";
                string clientUserName = "";
                E_OrderPayWay orderPayWay = E_OrderPayWay.Unknow;
                bool isPaySuccess = false;
                if (payPartner == E_OrderPayPartner.Wx)
                {
                    #region 解释微信支付协议
                    LogUtil.Info("解释微信支付协议");
                    orderPayWay = E_OrderPayWay.Wechat;

                    var dic = MyWeiXinSdk.CommonUtil.XmlToDictionary(content);
                    if (dic.ContainsKey("out_trade_no"))
                    {
                        orderSn = dic["out_trade_no"].ToString();
                    }

                    if (dic.ContainsKey("transaction_id"))
                    {
                        payPartnerOrderSn = dic["transaction_id"].ToString();
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
                else if (payPartner == E_OrderPayPartner.Ali)
                {
                    #region 解释支付宝支付协议
                    LogUtil.Info("解释支付宝支付协议");
                    orderPayWay = E_OrderPayWay.AliPay;


                    if (from == E_OrderNotifyLogNotifyFrom.OrderQuery)
                    {
                        TradeQueryResult result = Newtonsoft.Json.JsonConvert.DeserializeObject<TradeQueryResult>(content);
                        if (result != null)
                        {

                            var payResult = result.alipay_trade_query_response;
                            if (payResult != null)
                            {
                                if (payResult.code == "10000")
                                {
                                    if (!string.IsNullOrEmpty(payResult.out_trade_no))
                                    {
                                        orderSn = payResult.out_trade_no;
                                    }

                                    if (!string.IsNullOrEmpty(payResult.trade_no))
                                    {
                                        payPartnerOrderSn = payResult.trade_no;
                                    }

                                    if (!string.IsNullOrEmpty(payResult.buyer_logon_id))
                                    {
                                        clientUserName = payResult.buyer_logon_id;
                                    }

                                    LogUtil.Info("解释支付宝支付协议，订单号：" + orderSn);

                                    if (payResult.trade_status == "TRADE_SUCCESS")
                                    {
                                        isPaySuccess = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        var dic = MyAlipaySdk.CommonUtil.FormStringToDictionary(content);

                        if (dic.ContainsKey("out_trade_no"))
                        {
                            orderSn = dic["out_trade_no"].ToString();
                        }

                        if (dic.ContainsKey("trade_no"))
                        {
                            payPartnerOrderSn = dic["trade_no"].ToString();
                        }

                        LogUtil.Info("解释支付宝支付协议，订单号：" + orderSn);


                        if (dic.ContainsKey("buyer_logon_id"))
                        {
                            clientUserName = dic["buyer_logon_id"];
                        }


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
                else if (payPartner == E_OrderPayPartner.Tg)
                {
                    #region 解释通莞支付协议

                    if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<AllQrcodePayAsynNotifyResult>(content);
                        if (result != null)
                        {
                            if (result.state == "0")
                            {
                                isPaySuccess = true;
                                orderSn = result.lowOrderId;
                                payPartnerOrderSn = result.upOrderId;
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
                                    payPartnerOrderSn = result.upOrderId;
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


                if (isPaySuccess && !string.IsNullOrEmpty(orderSn))
                {
                    LogUtil.Info("解释支付协议结果，支付成功");

                    Dictionary<string, string> pms = new Dictionary<string, string>();
                    pms.Add("clientUserName", clientUserName);

                    PaySuccess(operater, orderSn, orderPayWay, DateTime.Now, pms);
                }


                var mod_OrderNotifyLog = new OrderNotifyLog();
                mod_OrderNotifyLog.Id = GuidUtil.New();

                var order = CurrentDb.Order.Where(m => m.Sn == orderSn).FirstOrDefault();

                if (order != null)
                {
                    mod_OrderNotifyLog.MerchId = order.MerchId;
                    mod_OrderNotifyLog.OrderId = order.Id;
                }

                mod_OrderNotifyLog.OrderSn = orderSn;
                mod_OrderNotifyLog.PayPartner = payPartner;
                mod_OrderNotifyLog.PayPartnerOrderSn = payPartnerOrderSn;
                mod_OrderNotifyLog.NotifyContent = content;
                mod_OrderNotifyLog.NotifyFrom = from;
                mod_OrderNotifyLog.NotifyType = E_OrderNotifyLogNotifyType.Pay;
                mod_OrderNotifyLog.CreateTime = DateTime.Now;
                mod_OrderNotifyLog.Creator = operater;
                CurrentDb.OrderNotifyLog.Add(mod_OrderNotifyLog);
                CurrentDb.SaveChanges();


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
                order.PayedTime = DateTime.Now;
                order.MendTime = DateTime.Now;
                order.Mender = operater;

                if (pms != null)
                {
                    if (pms.ContainsKey("clientUserName"))
                    {
                        if (!string.IsNullOrEmpty(pms["clientUserName"]))
                        {
                            order.ClientUserName = pms["clientUserName"];
                        }
                    }
                }


                var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == order.Id).ToList();

                foreach (var orderDetailsChildSon in orderDetailsChildSons)
                {
                    orderDetailsChildSon.Status = E_OrderDetailsChildSonStatus.WaitPickup;
                    orderDetailsChildSon.PayedTime = DateTime.Now;
                    orderDetailsChildSon.PayWay = payWay;
                    orderDetailsChildSon.Mender = GuidUtil.Empty();
                    orderDetailsChildSon.MendTime = DateTime.Now;
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

                Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPay, order.Id);

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, string.Format("支付完成通知：订单号({0})通知成功", orderSn));
            }

            return result;
        }
        public CustomJsonResult<RetPayResultQuery> PayResultQuery(string operater, string orderId)
        {
            var result = new CustomJsonResult<RetPayResultQuery>();

            var order = BizFactory.Order.GetOne(orderId);

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

                if (order.Status == E_OrderStatus.Canceled)
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
                    order.Status = E_OrderStatus.Canceled;
                    order.Mender = GuidUtil.Empty();
                    order.MendTime = DateTime.Now;
                    order.CanceledTime = DateTime.Now;
                    order.CancelReason = cancelReason;

                    var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderDetailsChildSons)
                    {
                        item.Status = E_OrderDetailsChildSonStatus.Canceled;
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

                    Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPay, order.Id);

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


                switch (rop.PayPartner)
                {
                    case E_OrderPayPartner.Wx:
                        #region  Wechat
                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.WxByNt:
                                #region WechatByNt
                                order.PayPartner = E_OrderPayPartner.Wx;
                                order.PayWay = E_OrderPayWay.Wechat;
                                var wechatByNative_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);
                                var wechatByNative_UnifiedOrder = SdkFactory.Wx.UnifiedOrderByNative(wechatByNative_AppInfoConfig, order.MerchId, order.StoreId, order.Sn, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);
                                if (string.IsNullOrEmpty(wechatByNative_UnifiedOrder.PrepayId))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var wechatByNative_PayParams = new { PayUrl = wechatByNative_UnifiedOrder.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", wechatByNative_PayParams);
                                #endregion
                                break;
                            case E_OrderPayCaller.WxByMp:
                                #region WechatByMp
                                order.PayPartner = E_OrderPayPartner.Wx;
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

                                var pms = SdkFactory.Wx.GetJsApiPayParams(wechatByMp_AppInfoConfig, order.Id, order.Sn, wechatByMp_UnifiedOrder.PrepayId);

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", pms);
                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);

                        }
                        #endregion 
                        break;
                    case E_OrderPayPartner.Ali:
                        #region AliPay
                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.AliByNt:
                                #region AlipayByNt
                                order.PayPartner = E_OrderPayPartner.Ali;
                                order.PayWay = E_OrderPayWay.AliPay;
                                var alipayByNative_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetAlipayMpAppInfoConfig(order.MerchId);
                                var alipayByNative_UnifiedOrder = SdkFactory.AliPay.UnifiedOrderByNative(alipayByNative_AppInfoConfig, order.MerchId, order.StoreId, order.Sn, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);
                                if (string.IsNullOrEmpty(alipayByNative_UnifiedOrder.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var alipayByNative_PayParams = new { PayUrl = alipayByNative_UnifiedOrder.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", alipayByNative_PayParams);
                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                        }
                        #endregion
                        break;
                    case E_OrderPayPartner.Tg:
                        #region 通莞支付

                        var tgPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetTgPayInfoConfg(order.MerchId);

                        order.PayPartner = E_OrderPayPartner.Tg;

                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.AggregatePayByNt:
                                #region AggregatePayByNt

                                decimal chargeAmount = order.ChargeAmount;
                                if (ConfigurationManager.AppSettings["custom:IsPayTest"] != null)
                                {
                                    if (ConfigurationManager.AppSettings["custom:IsPayTest"] == "true")
                                    {
                                        chargeAmount = 0.01m;
                                    }
                                }

                                var tgPay_AllQrcodePay = SdkFactory.TgPay.AllQrcodePay(tgPayInfoConfig, order.MerchId, order.StoreId, order.Sn, chargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", orderAttach, order.PayExpireTime.Value);
                                if (string.IsNullOrEmpty(tgPay_AllQrcodePay.codeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var tg_AllQrcodePay_PayParams = new { PayUrl = tgPay_AllQrcodePay.codeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", tg_AllQrcodePay_PayParams);

                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                        }
                        #endregion 
                        break;
                    case E_OrderPayPartner.Xrt:
                        #region Xrt支付

                        var xrtPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetXrtPayInfoConfg(order.MerchId);

                        order.PayPartner = E_OrderPayPartner.Xrt;

                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.AggregatePayByNt:
                                #region AggregatePayByNt

                                decimal chargeAmount = order.ChargeAmount;
                                if (ConfigurationManager.AppSettings["custom:IsPayTest"] != null)
                                {
                                    if (ConfigurationManager.AppSettings["custom:IsPayTest"] == "true")
                                    {
                                        chargeAmount = 0.01m;
                                    }
                                }

                                var xrtPay_WxPayBuildByNtResult = SdkFactory.XrtPay.WxPayBuildByNt(xrtPayInfoConfig, order.MerchId, order.StoreId, "", order.Sn, chargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);

                                if (string.IsNullOrEmpty(xrtPay_WxPayBuildByNtResult.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var xrtPay_WxPayBuildByNtResultParams = new { PayUrl = xrtPay_WxPayBuildByNtResult.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", xrtPay_WxPayBuildByNtResultParams);

                                #endregion
                                break;
                            default:
                                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                        }
                        #endregion
                        break;
                    default:
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "暂时不支持该方式支付", null);
                }

                Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPay, order.Id, order.PayExpireTime.Value, order);

                CurrentDb.SaveChanges();
                ts.Complete();
            }

            return result;
        }
        public OrderDetailsByPickupModel GetOrderDetailsByPickup(string orderId, string machineId)
        {
            var model = new OrderDetailsByPickupModel();
            var order = BizFactory.Order.GetOne(orderId);
            var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == orderId && m.SellChannelRefId == machineId && m.SellChannelRefType == E_SellChannelRefType.Machine).ToList();
            var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == orderId).ToList();

            model.OrderId = order.Id;
            model.OrderSn = order.Sn;

            foreach (var orderDetailsChild in orderDetailsChilds)
            {
                var sku = new OrderDetailsByPickupModel.ProductSku();
                sku.Id = orderDetailsChild.PrdProductSkuId;
                sku.Name = orderDetailsChild.PrdProductSkuName;
                sku.MainImgUrl = orderDetailsChild.PrdProductSkuMainImgUrl;
                sku.Quantity = orderDetailsChild.Quantity;


                var l_orderDetailsChildSons = orderDetailsChildSons.Where(m => m.OrderDetailsChildId == orderDetailsChild.Id && m.PrdProductSkuId == orderDetailsChild.PrdProductSkuId).ToList();

                sku.QuantityBySuccess = l_orderDetailsChildSons.Where(m => m.Status == E_OrderDetailsChildSonStatus.Completed).Count();

                foreach (var orderDetailsChildSon in l_orderDetailsChildSons)
                {
                    var slot = new OrderDetailsByPickupModel.Slot();
                    slot.UniqueId = orderDetailsChildSon.Id;
                    slot.SlotId = orderDetailsChildSon.SlotId;
                    slot.Status = orderDetailsChildSon.Status;

                    if (order.Status == E_OrderStatus.Payed)
                    {
                        if (orderDetailsChildSon.Status == E_OrderDetailsChildSonStatus.WaitPickup)
                        {
                            slot.IsAllowPickup = true;
                        }
                    }

                    sku.Slots.Add(slot);
                }

                model.ProductSkus.Add(sku);
            }

            return model;
        }

        public string GetPayWayName(E_OrderPayWay payWay)
        {
            string str = "";
            switch (payWay)
            {
                case E_OrderPayWay.Wechat:
                    str = "微信支付";
                    break;
                case E_OrderPayWay.AliPay:
                    str = "支付宝";
                    break;
                default:
                    str = "未知";
                    break;
            }

            return str;
        }

        public string GetTradeTypeName(E_RptOrderTradeType tradeTyp)
        {
            string str = "";
            switch (tradeTyp)
            {
                case E_RptOrderTradeType.Pay:
                    str = "交易";
                    break;
                case E_RptOrderTradeType.Refund:
                    str = "退款";
                    break;
                default:
                    str = "未知";
                    break;
            }

            return str;
        }
    }
}
