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

                    var buildOrderSubs = BuildOrderSubs(rop.ProductSkus, bizProductSkus);

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
                    order.PayStatus = E_OrderPayStatus.WaitPay;
                    order.Source = rop.Source;
                    order.PickupCode = RedisSnUtil.BuildPickupCode();
                    order.IsTestMode = rop.IsTestMode;

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

                    if (rop.IsTestMode)
                    {
                        order.OriginalAmount = buildOrderSubs.Sum(m => m.OriginalAmount);
                        order.DiscountAmount = buildOrderSubs.Sum(m => m.DiscountAmount);
                        order.ChargeAmount = 0.01m;
                    }
                    else
                    {
                        order.OriginalAmount = buildOrderSubs.Sum(m => m.OriginalAmount);
                        order.DiscountAmount = buildOrderSubs.Sum(m => m.DiscountAmount);
                        order.ChargeAmount = buildOrderSubs.Sum(m => m.ChargeAmount);
                    }


                    order.SellChannelRefIds = string.Join(",", buildOrderSubs.Select(m => m.SellChannelRefId).ToArray());

                    foreach (var buildOrderSub in buildOrderSubs)
                    {
                        var orderSub= new OrderSub();
                        orderSub.Id = GuidUtil.New();
                        orderSub.Sn = order.Sn + buildOrderSubs.IndexOf(buildOrderSub).ToString();
                        orderSub.ClientUserId = rop.ClientUserId;
                        orderSub.MerchId = store.MerchId;
                        orderSub.StoreId = rop.StoreId;
                        orderSub.StoreName = store.Name;
                        orderSub.SellChannelRefType = buildOrderSub.SellChannelRefType;
                        orderSub.SellChannelRefId = buildOrderSub.SellChannelRefId;

                        switch (buildOrderSub.SellChannelRefType)
                        {
                            case E_SellChannelRefType.Machine:
                                orderSub.SellChannelRefName = "【机器自提】 " + BizFactory.Merch.GetMachineName(order.MerchId, buildOrderSub.SellChannelRefId);
                                orderSub.SellChannelRefType = E_SellChannelRefType.Machine;
                                orderSub.SellChannelRefId = buildOrderSub.SellChannelRefId;
                                orderSub.Receiver = null;
                                orderSub.ReceiverPhone = null;
                                orderSub.ReceptionAddress = store.Address;
                                break;
                        }
                        orderSub.OrderId = order.Id;
                        orderSub.OrderSn = order.Sn;
                        orderSub.OriginalAmount = buildOrderSub.OriginalAmount;
                        orderSub.DiscountAmount = buildOrderSub.DiscountAmount;
                        orderSub.ChargeAmount = buildOrderSub.ChargeAmount;
                        orderSub.Quantity = buildOrderSub.Quantity;
                        orderSub.Creator = operater;
                        orderSub.CreateTime = DateTime.Now;
                        CurrentDb.OrderSub.Add(orderSub);

                        foreach (var buildOrderSubChid in buildOrderSub.Childs)
                        {
                            var productSku = bizProductSkus.Where(m => m.Id == buildOrderSubChid.ProductSkuId).FirstOrDefault();

                            var orderSubChild = new OrderSubChild();
                            orderSubChild.Id = GuidUtil.New();
                            orderSubChild.Sn = orderSub.Sn + buildOrderSub.Childs.IndexOf(buildOrderSubChid).ToString();
                            orderSubChild.ClientUserId = rop.ClientUserId;
                            orderSubChild.MerchId = store.MerchId;
                            orderSubChild.StoreId = rop.StoreId;
                            orderSubChild.SellChannelRefType = buildOrderSubChid.SellChannelRefType;
                            orderSubChild.SellChannelRefId = buildOrderSubChid.SellChannelRefId;
                            orderSubChild.SellChannelRefName = orderSub.SellChannelRefName;
                            orderSubChild.OrderId = order.Id;
                            orderSubChild.OrderSn = order.Sn;
                            orderSubChild.OrderSubId = orderSub.Id;
                            orderSubChild.OrderSubSn = orderSub.Sn;
                            orderSubChild.PrdProductSkuId = buildOrderSubChid.ProductSkuId;
                            orderSubChild.PrdProductId = buildOrderSubChid.ProductId;
                            orderSubChild.PrdProductSkuName = productSku.Name;
                            orderSubChild.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                            orderSubChild.PrdProductSkuSpecDes = productSku.SpecDes;
                            orderSubChild.PrdProductSkuProducer = productSku.Producer;
                            orderSubChild.PrdProductSkuBarCode = productSku.BarCode;
                            orderSubChild.PrdProductSkuCumCode = productSku.CumCode;
                            orderSubChild.SalePrice = buildOrderSubChid.SalePrice;
                            orderSubChild.SalePriceByVip = buildOrderSubChid.SalePriceByVip;
                            orderSubChild.Quantity = buildOrderSubChid.Quantity;
                            orderSubChild.OriginalAmount = buildOrderSubChid.OriginalAmount;
                            orderSubChild.DiscountAmount = buildOrderSubChid.DiscountAmount;
                            orderSubChild.ChargeAmount = buildOrderSubChid.ChargeAmount;
                            orderSubChild.Creator = operater;
                            orderSubChild.CreateTime = DateTime.Now;
                            CurrentDb.OrderSubChild.Add(orderSubChild);

                            foreach (var buildOrderSubChidUnique in buildOrderSubChid.Uniques)
                            {
                                var orderSubChildUnique = new OrderSubChildUnique();
                                orderSubChildUnique.Id = GuidUtil.New();
                                orderSubChildUnique.Sn = orderSubChild.Sn + buildOrderSubChid.Uniques.IndexOf(buildOrderSubChidUnique);
                                orderSubChildUnique.ClientUserId = rop.ClientUserId;
                                orderSubChildUnique.MerchId = store.MerchId;
                                orderSubChildUnique.StoreId = rop.StoreId;
                                orderSubChildUnique.StoreName = order.StoreName;
                                orderSubChildUnique.SellChannelRefType = buildOrderSubChidUnique.SellChannelRefType;
                                orderSubChildUnique.SellChannelRefId = buildOrderSubChidUnique.SellChannelRefId;
                                orderSubChildUnique.SellChannelRefName = orderSubChild.SellChannelRefName;
                                orderSubChildUnique.OrderId = order.Id;
                                orderSubChildUnique.OrderSn = order.Sn;
                                orderSubChildUnique.OrderSubId = orderSub.Id;
                                orderSubChildUnique.OrderSubSn = orderSub.Sn;
                                orderSubChildUnique.OrderSubChildId = orderSubChild.Id;
                                orderSubChildUnique.OrderSubChildSn = orderSubChild.Sn;
                                orderSubChildUnique.SlotId = buildOrderSubChidUnique.SlotId;
                                orderSubChildUnique.PrdProductSkuId = buildOrderSubChidUnique.ProductSkuId;
                                orderSubChildUnique.PrdProductId = orderSubChild.PrdProductId;
                                orderSubChildUnique.PrdProductSkuName = productSku.Name;
                                orderSubChildUnique.PrdProductSkuMainImgUrl = productSku.MainImgUrl;
                                orderSubChildUnique.PrdProductSkuSpecDes = productSku.SpecDes;
                                orderSubChildUnique.PrdProductSkuProducer = productSku.Producer;
                                orderSubChildUnique.PrdProductSkuBarCode = productSku.BarCode;
                                orderSubChildUnique.PrdProductSkuCumCode = productSku.CumCode;
                                orderSubChildUnique.SalePrice = buildOrderSubChidUnique.SalePrice;
                                orderSubChildUnique.SalePriceByVip = buildOrderSubChidUnique.SalePriceByVip;
                                orderSubChildUnique.Quantity = buildOrderSubChidUnique.Quantity;
                                orderSubChildUnique.OriginalAmount = buildOrderSubChidUnique.OriginalAmount;
                                orderSubChildUnique.DiscountAmount = buildOrderSubChidUnique.DiscountAmount;
                                orderSubChildUnique.ChargeAmount = buildOrderSubChidUnique.ChargeAmount;
                                orderSubChildUnique.Creator = operater;
                                orderSubChildUnique.CreateTime = DateTime.Now;
                                orderSubChildUnique.Status = E_OrderPickupStatus.WaitPay;
                                CurrentDb.OrderSubChildUnique.Add(orderSubChildUnique);
                            }

                            foreach (var slotStock in buildOrderSubChid.SlotStock)
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
        private List<BuildOrderSub> BuildOrderSubs(List<RopOrderReserve.ProductSku> reserveDetails, List<ProductSkuInfoAndStockModel> productSkus)
        {
            List<BuildOrderSub> buildOrderSubs = new List<BuildOrderSub>();

            List<BuildOrderSub.Unique> buildOrderSubUniques = new List<BuildOrderSub.Unique>();

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
                            int reservedQuantity = buildOrderSubUniques.Where(m => m.ProductSkuId == reserveDetail.Id && m.SellChannelRefType == channelType).Sum(m => m.Quantity);//已订的数量
                            int needReserveQuantity = reserveDetail.Quantity;//需要订的数量
                            if (reservedQuantity != needReserveQuantity)
                            {
                                var buildOrderSubUnique = new BuildOrderSub.Unique();
                                buildOrderSubUnique.Id = GuidUtil.New();
                                buildOrderSubUnique.SellChannelRefType = item.RefType;
                                buildOrderSubUnique.SellChannelRefId = item.RefId;
                                buildOrderSubUnique.ReceptionMode = receptionMode;
                                buildOrderSubUnique.ProductSkuId = productSku.Id;
                                buildOrderSubUnique.ProductId = productSku.ProductId;
                                buildOrderSubUnique.SlotId = item.SlotId;
                                buildOrderSubUnique.Quantity = 1;
                                buildOrderSubUnique.SalePrice = productSku_Stocks[0].SalePrice;
                                buildOrderSubUnique.SalePriceByVip = productSku_Stocks[0].SalePriceByVip;
                                buildOrderSubUnique.OriginalAmount = buildOrderSubUnique.Quantity * productSku_Stocks[0].SalePrice;
                                buildOrderSubUniques.Add(buildOrderSubUnique);
                            }
                        }
                    }
                }
            }

            var sumOriginalAmount = buildOrderSubUniques.Sum(m => m.OriginalAmount);
            var sumDiscountAmount = 0m;
            for (int i = 0; i < buildOrderSubUniques.Count; i++)
            {
                decimal scale = (sumOriginalAmount == 0 ? 0 : (buildOrderSubUniques[i].OriginalAmount / sumOriginalAmount));
                buildOrderSubUniques[i].DiscountAmount = Decimal.Round(scale * sumDiscountAmount, 2);
                buildOrderSubUniques[i].ChargeAmount = buildOrderSubUniques[i].OriginalAmount - buildOrderSubUniques[i].DiscountAmount;
            }

            var sumDiscountAmount2 = buildOrderSubUniques.Sum(m => m.DiscountAmount);
            if (sumDiscountAmount != sumDiscountAmount2)
            {
                var diff = sumDiscountAmount - sumDiscountAmount2;

                buildOrderSubUniques[buildOrderSubUniques.Count - 1].DiscountAmount = buildOrderSubUniques[buildOrderSubUniques.Count - 1].DiscountAmount + diff;
                buildOrderSubUniques[buildOrderSubUniques.Count - 1].ChargeAmount = buildOrderSubUniques[buildOrderSubUniques.Count - 1].OriginalAmount - buildOrderSubUniques[buildOrderSubUniques.Count - 1].DiscountAmount;
            }


            var detailGroups = (from c in buildOrderSubUniques
                                select new
                                {
                                    c.ReceptionMode,
                                    c.SellChannelRefType,
                                    c.SellChannelRefId
                                }).Distinct().ToList();



            foreach (var detailGroup in detailGroups)
            {
                var buildOrderSub = new BuildOrderSub();
                buildOrderSub.ReceptionMode = detailGroup.ReceptionMode;
                buildOrderSub.SellChannelRefType = detailGroup.SellChannelRefType;
                buildOrderSub.SellChannelRefId = detailGroup.SellChannelRefId;
                buildOrderSub.Quantity = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.Quantity);
                buildOrderSub.OriginalAmount = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.OriginalAmount);
                buildOrderSub.DiscountAmount = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.DiscountAmount);
                buildOrderSub.ChargeAmount = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailGroup.SellChannelRefId).Sum(m => m.ChargeAmount);


                var detailChildGroups = (from c in buildOrderSubUniques
                                         where c.SellChannelRefId == detailGroup.SellChannelRefId
                                         select new
                                         {
                                             c.SellChannelRefType,
                                             c.SellChannelRefId,
                                             c.ProductSkuId
                                         }).Distinct().ToList();

                foreach (var detailChildGroup in detailChildGroups)
                {

                    var orderSubChild = new BuildOrderSub.Child();
                    orderSubChild.SellChannelRefType = detailChildGroup.SellChannelRefType;
                    orderSubChild.SellChannelRefId = detailChildGroup.SellChannelRefId;
                    orderSubChild.ProductSkuId = detailChildGroup.ProductSkuId;
                    orderSubChild.ProductId = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().ProductId;
                    orderSubChild.SalePrice = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().SalePrice;
                    orderSubChild.SalePriceByVip = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).First().SalePriceByVip;
                    orderSubChild.Quantity = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.Quantity);
                    orderSubChild.OriginalAmount = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.OriginalAmount);
                    orderSubChild.DiscountAmount = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.DiscountAmount);
                    orderSubChild.ChargeAmount = buildOrderSubUniques.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.ProductSkuId == detailChildGroup.ProductSkuId).Sum(m => m.ChargeAmount);

                    var detailChildSonGroups = (from c in buildOrderSubUniques
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
                        var orderSubDetailUnit = new BuildOrderSub.Unique();
                        orderSubDetailUnit.Id = detailChildSonGroup.Id;
                        orderSubDetailUnit.SellChannelRefType = detailChildSonGroup.SellChannelRefType;
                        orderSubDetailUnit.SellChannelRefId = detailChildSonGroup.SellChannelRefId;
                        orderSubDetailUnit.ReceptionMode = detailChildSonGroup.ReceptionMode;
                        orderSubDetailUnit.ProductSkuId = detailChildSonGroup.ProductSkuId;
                        orderSubDetailUnit.SlotId = detailChildSonGroup.SlotId;
                        orderSubDetailUnit.Quantity = detailChildSonGroup.Quantity;
                        orderSubDetailUnit.SalePrice = detailChildSonGroup.SalePrice;
                        orderSubDetailUnit.SalePriceByVip = detailChildSonGroup.SalePriceByVip;
                        orderSubDetailUnit.OriginalAmount = detailChildSonGroup.OriginalAmount;
                        orderSubDetailUnit.DiscountAmount = detailChildSonGroup.DiscountAmount;
                        orderSubDetailUnit.ChargeAmount = detailChildSonGroup.ChargeAmount;
                        orderSubChild.Uniques.Add(orderSubDetailUnit);
                    }



                    var slotStockGroups = (from c in buildOrderSubUniques
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
                        var slotStock = new BuildOrderSub.SlotStock();
                        slotStock.SellChannelRefType = slotStockGroup.SellChannelRefType;
                        slotStock.SellChannelRefId = slotStockGroup.SellChannelRefId;
                        slotStock.ProductSkuId = slotStockGroup.ProductSkuId;
                        slotStock.SlotId = slotStockGroup.SlotId;
                        slotStock.Quantity = buildOrderSubUniques.Where(m => m.SellChannelRefType == slotStockGroup.SellChannelRefType && m.SellChannelRefId == slotStockGroup.SellChannelRefId && m.ProductSkuId == slotStockGroup.ProductSkuId && m.SlotId == slotStockGroup.SlotId).Sum(m => m.Quantity);
                        orderSubChild.SlotStock.Add(slotStock);
                    }

                    buildOrderSub.Childs.Add(orderSubChild);

                }

                buildOrderSubs.Add(buildOrderSub);
            }

            return buildOrderSubs;
        }
        private static readonly object lock_PayResultNotify = new object();
        public CustomJsonResult PayResultNotify(string operater, E_OrderPayPartner payPartner, E_OrderNotifyLogNotifyFrom from, string content)
        {
            LogUtil.Info("PayResultNotify");
            lock (lock_PayResultNotify)
            {
                var payResult = new PayResult();

                if (payPartner == E_OrderPayPartner.Wx)
                {
                    #region 解释微信支付协议
                    LogUtil.Info("解释微信支付协议");

                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.Wx.Convert2PayResultByPayQuery(null, content);
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.Wx.Convert2PayResultByNotifyUrl(null, content);
                    }
                    #endregion
                }
                else if (payPartner == E_OrderPayPartner.Ali)
                {
                    #region 解释支付宝支付协议
                    LogUtil.Info("解释支付宝支付协议");

                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.AliPay.Convert2PayResultByPayQuery(null, content);
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.AliPay.Convert2PayResultByNotifyUrl(null, content);
                    }

                    #endregion
                }
                else if (payPartner == E_OrderPayPartner.Tg)
                {
                    #region 解释通莞支付协议

                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.TgPay.Convert2PayResultByPayQuery(null, content);
                    }

                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.TgPay.Convert2PayResultByNotifyUrl(null, content);
                    }
                    #endregion
                }
                else if (payPartner == E_OrderPayPartner.Xrt)
                {
                    #region 解释XRT支付协议
                    if (from == E_OrderNotifyLogNotifyFrom.PayQuery)
                    {
                        payResult = SdkFactory.XrtPay.Convert2PayResultByPayQuery(null, content);
                    }
                    else if (from == E_OrderNotifyLogNotifyFrom.NotifyUrl)
                    {
                        payResult = SdkFactory.XrtPay.Convert2PayResultByNotifyUrl(null, content);
                    }
                    #endregion
                }

                if (payResult.IsPaySuccess)
                {
                    LogUtil.Info("解释支付协议结果，支付成功");
                    Dictionary<string, string> pms = new Dictionary<string, string>();
                    pms.Add("clientUserName", payResult.ClientUserName);

                    PaySuccess(operater, payResult.OrderSn, payResult.OrderPayWay, DateTime.Now, pms);
                }


                var mod_OrderNotifyLog = new OrderNotifyLog();
                mod_OrderNotifyLog.Id = GuidUtil.New();

                var order = CurrentDb.Order.Where(m => m.Sn == payResult.OrderSn).FirstOrDefault();

                if (order != null)
                {
                    mod_OrderNotifyLog.MerchId = order.MerchId;
                    mod_OrderNotifyLog.OrderId = order.Id;
                }

                mod_OrderNotifyLog.OrderSn = payResult.OrderSn;
                mod_OrderNotifyLog.PayPartner = payPartner;
                mod_OrderNotifyLog.PayPartnerOrderSn = payResult.PayPartnerOrderSn;
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

                //if (order.Status != E_OrderStatus.WaitPay)
                //{
                //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, string.Format("找不到该订单号({0})", orderSn));
                //}

                order.PayWay = payWay;
                order.PayStatus = E_OrderPayStatus.PaySuccess;

                if (order.Status == E_OrderStatus.WaitPay)
                {
                    order.Status = E_OrderStatus.Payed;

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

                    var oderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var oderSubChildUnique in oderSubChildUniques)
                    {
                        oderSubChildUnique.Status = E_OrderPickupStatus.WaitPickup;
                        oderSubChildUnique.PayedTime = DateTime.Now;
                        oderSubChildUnique.PayStatus = E_OrderPayStatus.PaySuccess;
                        oderSubChildUnique.PayWay = payWay;
                        oderSubChildUnique.Mender = GuidUtil.Empty();
                        oderSubChildUnique.MendTime = DateTime.Now;
                    }

                    var childSons = (
                        from q in oderSubChildUniques
                        group q by new { q.PrdProductSkuId, q.Quantity, q.SellChannelRefType, q.SlotId, q.SellChannelRefId } into b
                        select new { b.Key.PrdProductSkuId, b.Key.SellChannelRefId, b.Key.SellChannelRefType, b.Key.SlotId, Quantity = b.Sum(c => c.Quantity) }).ToList();

                    foreach (var item in childSons)
                    {
                        BizFactory.ProductSku.OperateStockQuantity(operater, OperateStockType.OrderPaySuccess, order.MerchId, order.StoreId, item.SellChannelRefId, item.SlotId, item.PrdProductSkuId, item.Quantity);
                    }
                }

                order.PayedTime = DateTime.Now;
                order.MendTime = DateTime.Now;
                order.Mender = operater;

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

                    var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderSubChildUniques)
                    {
                        item.Status = E_OrderPickupStatus.Canceled;
                        item.Mender = GuidUtil.Empty();
                        item.MendTime = DateTime.Now;
                    }

                    var childSons = (
                        from q in orderSubChildUniques
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
                        #region  Wechat支付
                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.WxByNt:
                                #region WechatByNt
                                order.PayPartner = E_OrderPayPartner.Wx;
                                order.PayWay = E_OrderPayWay.Wechat;
                                order.PayStatus = E_OrderPayStatus.Paying;
                                var wechatByNative_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);
                                var wechatByNative_UnifiedOrder = SdkFactory.Wx.PayBuildQrCode(wechatByNative_AppInfoConfig, E_OrderPayCaller.WxByNt, order.MerchId, order.StoreId, "", order.Sn, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);
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
                                order.PayStatus = E_OrderPayStatus.Paying;
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
                        #region AliPay支付
                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.AliByNt:
                                #region AlipayByNt
                                order.PayPartner = E_OrderPayPartner.Ali;
                                order.PayWay = E_OrderPayWay.AliPay;
                                order.PayStatus = E_OrderPayStatus.Paying;
                                var alipayByNative_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetAlipayMpAppInfoConfig(order.MerchId);
                                var alipayByNative_UnifiedOrder = SdkFactory.AliPay.PayBuildQrCode(alipayByNative_AppInfoConfig, E_OrderPayCaller.AliByNt, order.MerchId, order.StoreId, "", order.Sn, 0.01m, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);
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
                        #region Tg支付

                        var tgPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetTgPayInfoConfg(order.MerchId);

                        order.PayPartner = E_OrderPayPartner.Tg;
                        order.PayStatus = E_OrderPayStatus.Paying;

                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.AggregatePayByNt:
                                #region AggregatePayByNt

                                var tgPay_AllQrcodePay = SdkFactory.TgPay.PayBuildQrCode(tgPayInfoConfig, rop.PayCaller, order.MerchId, order.StoreId, "", order.Sn, order.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);
                                if (string.IsNullOrEmpty(tgPay_AllQrcodePay.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var tg_AllQrcodePay_PayParams = new { PayUrl = tgPay_AllQrcodePay.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

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

                        // todo 发布去掉

                        var xrtPayInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetXrtPayInfoConfg(order.MerchId);

                        order.PayPartner = E_OrderPayPartner.Xrt;
                        order.PayStatus = E_OrderPayStatus.Paying;

                        switch (rop.PayCaller)
                        {
                            case E_OrderPayCaller.WxByNt:
                                #region WxByNt

                                var xrtPay_WxPayBuildByNtResult = SdkFactory.XrtPay.PayBuildQrCode(xrtPayInfoConfig, rop.PayCaller, order.MerchId, order.StoreId, "", order.Sn, order.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);

                                if (string.IsNullOrEmpty(xrtPay_WxPayBuildByNtResult.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var xrtPay_WxPayBuildByNtResultParams = new { PayUrl = xrtPay_WxPayBuildByNtResult.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", xrtPay_WxPayBuildByNtResultParams);

                                #endregion
                                break;
                            case E_OrderPayCaller.AliByNt:
                                #region AliByNt

                                var xrtPay_AliPayBuildByNtResult = SdkFactory.XrtPay.PayBuildQrCode(xrtPayInfoConfig, rop.PayCaller, order.MerchId, order.StoreId, "", order.Sn, order.ChargeAmount, "", Lumos.CommonUtil.GetIP(), "自助商品", order.PayExpireTime.Value);

                                if (string.IsNullOrEmpty(xrtPay_AliPayBuildByNtResult.CodeUrl))
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "支付二维码生成失败");
                                }

                                var xrtPay_AliPayBuildByNtResultParams = new { PayUrl = xrtPay_AliPayBuildByNtResult.CodeUrl, ChargeAmount = order.ChargeAmount.ToF2Price() };

                                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功", xrtPay_AliPayBuildByNtResultParams);

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
            var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == orderId && m.SellChannelRefId == machineId && m.SellChannelRefType == E_SellChannelRefType.Machine).ToList();
            var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderId == orderId).ToList();

            model.OrderId = order.Id;
            model.OrderSn = order.Sn;

            foreach (var orderSubChild in orderSubChilds)
            {
                var sku = new OrderDetailsByPickupModel.ProductSku();
                sku.Id = orderSubChild.PrdProductSkuId;
                sku.Name = orderSubChild.PrdProductSkuName;
                sku.MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl;
                sku.Quantity = orderSubChild.Quantity;


                var l_orderSubChildUniques = orderSubChildUniques.Where(m => m.OrderSubChildId == orderSubChild.Id && m.PrdProductSkuId == orderSubChild.PrdProductSkuId).ToList();

                sku.QuantityBySuccess = l_orderSubChildUniques.Where(m => m.Status == E_OrderPickupStatus.Completed).Count();

                foreach (var orderSubChildUnique in l_orderSubChildUniques)
                {
                    var slot = new OrderDetailsByPickupModel.Slot();
                    slot.UniqueId = orderSubChildUnique.Id;
                    slot.SlotId = orderSubChildUnique.SlotId;
                    slot.Status = orderSubChildUnique.Status;

                    if (order.Status == E_OrderStatus.Payed)
                    {
                        if (orderSubChildUnique.Status == E_OrderPickupStatus.WaitPickup)
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
