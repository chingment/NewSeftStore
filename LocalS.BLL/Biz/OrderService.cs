using LocalS.BLL;
using LocalS.Entity;
using Lumos;
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
        public CustomJsonResult<RetOrderReserve> Reserve(string operater, RopOrderReserve rop)
        {
            CustomJsonResult<RetOrderReserve> result = new CustomJsonResult<RetOrderReserve>();

            if (rop.ReserveMode == E_ReserveMode.Unknow)
            {
                return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "未知预定方式", null);
            }

            if (rop.ReserveMode == E_ReserveMode.OffLine)
            {
                if (string.IsNullOrEmpty(rop.SellChannelRefId))
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "机器ID不能为空", null);
                }
            }

            using (TransactionScope ts = new TransactionScope())
            {
                RetOrderReserve ret = new RetOrderReserve();

                var skuIds = rop.ProductSkus.Select(m => m.Id).ToArray();

                //检查是否有可买的商品

                List<StoreSellChannelStock> skusByStock = new List<StoreSellChannelStock>();

                if (rop.ReserveMode == E_ReserveMode.OffLine)
                {
                    skusByStock = CurrentDb.StoreSellChannelStock.Where(m => m.StoreId == rop.StoreId && m.RefType == E_StoreSellChannelRefType.Machine && m.RefId == rop.SellChannelRefId && skuIds.Contains(m.ProductSkuId)).ToList();
                }
                else if (rop.ReserveMode == E_ReserveMode.Online)
                {
                    skusByStock = CurrentDb.StoreSellChannelStock.Where(m => m.StoreId == rop.StoreId && skuIds.Contains(m.ProductSkuId)).ToList();
                }

                List<string> warn_tips = new List<string>();

                foreach (var sku in rop.ProductSkus)
                {
                    var skuModel = CacheServiceFactory.ProductSku.GetModelById(sku.Id);

                    var sellQuantity = 0;

                    if (rop.ReserveMode == E_ReserveMode.OffLine)
                    {
                        sellQuantity = skusByStock.Where(m => m.ProductSkuId == sku.Id && m.RefType == E_StoreSellChannelRefType.Machine && m.RefId == rop.SellChannelRefId).Sum(m => m.SellQuantity);
                    }
                    else if (rop.ReserveMode == E_ReserveMode.Online)
                    {
                        if (sku.ReceptionMode == E_ReceptionMode.Machine)
                        {
                            sellQuantity = skusByStock.Where(m => m.ProductSkuId == sku.Id && m.RefType == E_StoreSellChannelRefType.Machine).Sum(m => m.SellQuantity);
                        }
                        else if (sku.ReceptionMode == E_ReceptionMode.Express)
                        {
                            sellQuantity = skusByStock.Where(m => m.ProductSkuId == sku.Id && m.RefType == E_StoreSellChannelRefType.Express).Sum(m => m.SellQuantity);
                        }
                    }

                    var hasOffSell = skusByStock.Where(m => m.ProductSkuId == sku.Id).Where(m => m.IsOffSell == true).FirstOrDefault();

                    if (hasOffSell == null)
                    {
                        if (sellQuantity < sku.Quantity)
                        {
                            warn_tips.Add(string.Format("{0}的可销售数量为{1}个", skuModel.Name, sellQuantity));
                        }
                    }
                    else
                    {
                        warn_tips.Add(string.Format("{0}已经下架", skuModel.Name));
                    }
                }

                if (warn_tips.Count > 0)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, string.Join(";", warn_tips.ToArray()), null);
                }

                var store = CurrentDb.Store.Where(m => m.Id == rop.StoreId).FirstOrDefault();
                if (store == null)
                {
                    return new CustomJsonResult<RetOrderReserve>(ResultType.Failure, ResultCode.Failure, "店铺无效", null);
                }

                var order = new Order();
                order.Id = GuidUtil.New();
                //order.Sn = SnUtil.Build(Enumeration.BizSnType.Order, store.MerchId);
                order.MerchId = store.MerchId;
                order.StoreId = rop.StoreId;
                order.StoreName = store.Name;
                order.ClientUserId = rop.ClientUserId;
                order.Quantity = rop.ProductSkus.Sum(m => m.Quantity);
                order.Status = E_OrderStatus.WaitPay;
                order.Source = rop.Source;
                order.SubmitTime = DateTime.Now;
                order.PayExpireTime = DateTime.Now.AddSeconds(300);
                order.Creator = operater;
                order.CreateTime = DateTime.Now;

                //todo 
                Random rd = new Random();
                int num = rd.Next(100000, 1000000);
                order.PickCode = num.ToString();

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

                var reserveDetails = GetReserveDetail(rop.ProductSkus, skusByStock);

                order.OriginalAmount = reserveDetails.Sum(m => m.OriginalAmount);
                order.DiscountAmount = reserveDetails.Sum(m => m.DiscountAmount);
                order.ChargeAmount = reserveDetails.Sum(m => m.ChargeAmount);

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
                        case E_StoreSellChannelRefType.Machine:
                            //var machine = CurrentDb.Machine.Where(m => m.Id == detail.ChannelId).FirstOrDefault();
                            //orderDetails.ChannelName = "【自提】 " + machine.Name;//todo 若 ChannelType 为1 则机器昵称，2为自取
                            break;
                        case E_StoreSellChannelRefType.Express:
                            orderDetails.SellChannelRefName = "【快递】";
                            break;
                    }
                    orderDetails.OrderId = order.Id;
                    orderDetails.OrderSn = order.Sn;
                    orderDetails.OriginalAmount = detail.OriginalAmount;
                    orderDetails.DiscountAmount = detail.DiscountAmount;
                    orderDetails.ChargeAmount = detail.ChargeAmount;
                    orderDetails.Quantity = detail.Quantity;
                    orderDetails.SubmitTime = DateTime.Now;
                    orderDetails.Creator = operater;
                    orderDetails.CreateTime = DateTime.Now;

                    //detail.MachineId为空 则为快递商品
                    if (detail.SellChannelRefId == GuidUtil.Empty())
                    {
                        orderDetails.Receiver = rop.Receiver;
                        orderDetails.ReceiverPhone = rop.ReceiverPhone;
                        orderDetails.ReceptionAddress = rop.ReceptionAddress;
                        orderDetails.SellChannelRefType = E_StoreSellChannelRefType.Express;
                        orderDetails.SellChannelRefId = GuidUtil.Empty();
                    }
                    else
                    {
                        orderDetails.Receiver = null;
                        orderDetails.ReceiverPhone = null;
                        orderDetails.ReceptionAddress = store.Address;
                        orderDetails.SellChannelRefType = E_StoreSellChannelRefType.Machine;
                        orderDetails.SellChannelRefId = detail.SellChannelRefId;
                    }

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
                        orderDetailsChild.OrderId = order.Id;
                        orderDetailsChild.OrderSn = order.Sn;
                        orderDetailsChild.OrderDetailsId = orderDetails.Id;
                        orderDetailsChild.OrderDetailsSn = orderDetails.Sn;
                        orderDetailsChild.ProductSkuId = detailsChild.SkuId;
                        orderDetailsChild.ProductSkuName = detailsChild.SkuName;
                        orderDetailsChild.ProductSkuImgUrl = detailsChild.SkuImgUrl;
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
                            orderDetailsChildSon.OrderId = order.Id;
                            orderDetailsChildSon.OrderSn = order.Sn;
                            orderDetailsChildSon.OrderDetailsId = orderDetails.Id;
                            orderDetailsChildSon.OrderDetailsSn = orderDetails.Sn;
                            orderDetailsChildSon.OrderDetailsChildId = orderDetailsChild.Id;
                            orderDetailsChildSon.OrderDetailsChildSn = orderDetailsChild.Sn;
                            orderDetailsChildSon.SlotId = detailsChildSon.SlotId;
                            orderDetailsChildSon.ProductSkuId = detailsChildSon.SkuId;
                            orderDetailsChildSon.ProductSkuName = detailsChildSon.SkuName;
                            orderDetailsChildSon.ProductSkuImgUrl = detailsChildSon.SkuImgUrl;
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
                            var machineStock = skusByStock.Where(m => m.ProductSkuId == slotStock.SkuId && m.SlotId == slotStock.SlotId && m.RefId == slotStock.SellChannelRefId).FirstOrDefault();

                            machineStock.LockQuantity += slotStock.Quantity;
                            machineStock.SellQuantity -= slotStock.Quantity;
                            machineStock.Mender = operater;
                            machineStock.MendTime = DateTime.Now;


                            var storeSellStockLog = new StoreSellChannelStockLog();
                            storeSellStockLog.Id = GuidUtil.New();
                            storeSellStockLog.MerchId = store.MerchId;
                            storeSellStockLog.StoreId = rop.StoreId;
                            storeSellStockLog.RefType = slotStock.SellChannelRefType;
                            storeSellStockLog.RefId = slotStock.SellChannelRefId;
                            storeSellStockLog.SlotId = slotStock.SlotId;
                            storeSellStockLog.ProductSkuId = slotStock.SkuId;
                           // storeSellStockLog.Quantity = machineStock.Quantity;
                            storeSellStockLog.LockQuantity = machineStock.LockQuantity;
                            storeSellStockLog.SellQuantity = machineStock.SellQuantity;
                            //  storeSellStockLog.ChangeType = Enumeration.MachineStockLogChangeTpye.Lock;
                            storeSellStockLog.ChangeQuantity = slotStock.Quantity;
                            storeSellStockLog.Creator = operater;
                            storeSellStockLog.CreateTime = DateTime.Now;
                            storeSellStockLog.RemarkByDev = string.Format("预定锁定库存：{0}", slotStock.Quantity);
                            CurrentDb.StoreSellChannelStockLog.Add(storeSellStockLog);
                        }
                    }
                }

                CurrentDb.Order.Add(order);

                ts.Complete();

                //Task4Factory.Global.Enter(TimerTaskType.CheckOrderPay, order.PayExpireTime.Value, order);

                ret.OrderId = order.Id;
                ret.OrderSn = order.Sn;

                result = new CustomJsonResult<RetOrderReserve>(ResultType.Success, ResultCode.Success, "预定成功", ret);

            }

            return result;

        }

        private List<OrderReserveDetail> GetReserveDetail(List<RopOrderReserve.ProductSku> reserveDetails, List<StoreSellChannelStock> storeSellStocks)
        {
            List<OrderReserveDetail> details = new List<OrderReserveDetail>();

            List<OrderReserveDetail.DetailChildSon> detailChildSons = new List<OrderReserveDetail.DetailChildSon>();

            var receptionModes = reserveDetails.Select(m => m.ReceptionMode).Distinct().ToArray();

            foreach (var receptionMode in receptionModes)
            {
                var l_reserveDetails = reserveDetails.Where(m => m.ReceptionMode == receptionMode).ToList();

                foreach (var reserveDetail in l_reserveDetails)
                {
                    E_StoreSellChannelRefType channelType = receptionMode == E_ReceptionMode.Express ? E_StoreSellChannelRefType.Express : E_StoreSellChannelRefType.Machine;
                    var l_storeSellStocks = storeSellStocks.Where(m => m.ProductSkuId == reserveDetail.Id && m.RefType == channelType).ToList();

                    foreach (var item in l_storeSellStocks)
                    {
                        for (var i = 0; i < item.SellQuantity; i++)
                        {
                            int reservedQuantity = detailChildSons.Where(m => m.SkuId == reserveDetail.Id && m.SellChannelRefType == channelType).Sum(m => m.Quantity);//已订的数量
                            int needReserveQuantity = reserveDetail.Quantity;//需要订的数量
                            if (reservedQuantity != needReserveQuantity)
                            {

                                var product = CacheServiceFactory.ProductSku.GetModelById(item.ProductSkuId);

                                var detailChildSon = new OrderReserveDetail.DetailChildSon();
                                detailChildSon.Id = GuidUtil.New();
                                detailChildSon.SellChannelRefType = item.RefType;
                                detailChildSon.SellChannelRefId = item.RefId;
                                detailChildSon.ReceptionMode = receptionMode;
                                detailChildSon.SkuId = item.ProductSkuId;
                                detailChildSon.SkuName = product.Name;
                                detailChildSon.SkuImgUrl = product.MainImgUrl;
                                detailChildSon.SlotId = item.SlotId;
                                detailChildSon.Quantity = 1;
                                detailChildSon.SalePrice = item.SalePrice;
                               // detailChildSon.SalePriceByVip = item.SalePriceByVip;
                                detailChildSon.OriginalAmount = detailChildSon.Quantity * item.SalePrice;
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
                                             c.SkuId
                                         }).Distinct().ToList();

                foreach (var detailChildGroup in detailChildGroups)
                {

                    var detailChild = new OrderReserveDetail.DetailChild();
                    detailChild.SellChannelRefType = detailChildGroup.SellChannelRefType;
                    detailChild.SellChannelRefId = detailChildGroup.SellChannelRefId;
                    detailChild.SkuId = detailChildGroup.SkuId;
                    detailChild.SkuName = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).First().SkuName;
                    detailChild.SkuImgUrl = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).First().SkuImgUrl;
                    detailChild.SalePrice = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).First().SalePrice;
                    detailChild.SalePriceByVip = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).First().SalePriceByVip;
                    detailChild.Quantity = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).Sum(m => m.Quantity);
                    detailChild.OriginalAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).Sum(m => m.OriginalAmount);
                    detailChild.DiscountAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).Sum(m => m.DiscountAmount);
                    detailChild.ChargeAmount = detailChildSons.Where(m => m.SellChannelRefId == detailChildGroup.SellChannelRefId && m.SkuId == detailChildGroup.SkuId).Sum(m => m.ChargeAmount);

                    var detailChildSonGroups = (from c in detailChildSons
                                                where c.SellChannelRefId == detailChildGroup.SellChannelRefId
                                             && c.SkuId == detailChildGroup.SkuId
                                                select new
                                                {
                                                    c.Id,
                                                    c.ReceptionMode,
                                                    c.SellChannelRefType,
                                                    c.SellChannelRefId,
                                                    c.SkuId,
                                                    c.SlotId,
                                                    c.Quantity,
                                                    c.SalePrice,
                                                    c.SalePriceByVip,
                                                    c.SkuImgUrl,
                                                    c.SkuName,
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
                        detailChildSon.SkuId = detailChildSonGroup.SkuId;
                        detailChildSon.SlotId = detailChildSonGroup.SlotId;
                        detailChildSon.Quantity = detailChildSonGroup.Quantity;
                        detailChildSon.SkuName = detailChildSonGroup.SkuName;
                        detailChildSon.SkuImgUrl = detailChildSonGroup.SkuImgUrl;
                        detailChildSon.SalePrice = detailChildSonGroup.SalePrice;
                        detailChildSon.SalePriceByVip = detailChildSonGroup.SalePriceByVip;
                        detailChildSon.OriginalAmount = detailChildSonGroup.OriginalAmount;
                        detailChildSon.DiscountAmount = detailChildSonGroup.DiscountAmount;
                        detailChildSon.ChargeAmount = detailChildSonGroup.ChargeAmount;
                        detailChild.Details.Add(detailChildSon);
                    }



                    var slotStockGroups = (from c in detailChildSons
                                           where c.SellChannelRefId == detailChildGroup.SellChannelRefId
                                        && c.SkuId == detailChildGroup.SkuId
                                           select new
                                           {
                                               c.SellChannelRefType,
                                               c.SellChannelRefId,
                                               c.SkuId,
                                               c.SlotId
                                           }).Distinct().ToList();


                    foreach (var slotStockGroup in slotStockGroups)
                    {
                        var slotStock = new OrderReserveDetail.SlotStock();
                        slotStock.SellChannelRefType = slotStockGroup.SellChannelRefType;
                        slotStock.SellChannelRefId = slotStockGroup.SellChannelRefId;
                        slotStock.SkuId = slotStockGroup.SkuId;
                        slotStock.SlotId = slotStockGroup.SlotId;
                        slotStock.Quantity = detailChildSons.Where(m => m.SellChannelRefType == slotStockGroup.SellChannelRefType && m.SellChannelRefId == slotStockGroup.SellChannelRefId && m.SkuId == slotStockGroup.SkuId && m.SlotId == slotStockGroup.SlotId).Sum(m => m.Quantity);
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
                //mod_OrderNotifyLog.NotifyFrom = from;
                //mod_OrderNotifyLog.NotifyType = E_OrderNotifyLogNotifyType.Pay;
                mod_OrderNotifyLog.CreateTime = DateTime.Now;
                mod_OrderNotifyLog.Creator = operater;
                //CurrentDb.OrderNotifyLog.Add(mod_OrderNotifyLog);
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

                order.Status = E_OrderStatus.Payed;
                order.PayTime = DateTime.Now;
                order.MendTime = DateTime.Now;
                order.Mender = operater;






                CurrentDb.SaveChanges();
                ts.Complete();


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

            result = new CustomJsonResult<RetPayResultQuery>(ResultType.Success, ResultCode.Success, "获取成功", ret);

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
                        //item.MendTime = this.DateTime;
                    }


                    var orderDetailsChilds = CurrentDb.OrderDetailsChild.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderDetailsChilds)
                    {
                        item.Status = E_OrderStatus.Cancled;
                        item.Mender = GuidUtil.Empty();
                        //item.MendTime = this.DateTime;
                    }

                    var orderDetailsChildSons = CurrentDb.OrderDetailsChildSon.Where(m => m.OrderId == order.Id).ToList();

                    foreach (var item in orderDetailsChildSons)
                    {
                        item.Status = E_OrderDetailsChildSonStatus.Cancled;
                        item.Mender = GuidUtil.Empty();
                        //item.MendTime = this.DateTime;

                        var machineStock = CurrentDb.StoreSellChannelStock.Where(m => m.MerchId == order.MerchId && m.StoreId == order.StoreId && m.ProductSkuId == item.ProductSkuId && m.SlotId == item.SlotId && m.RefId == item.SellChannelRefId && m.RefType ==  item.SellChannelRefType).FirstOrDefault();

                        machineStock.LockQuantity -= item.Quantity;
                        machineStock.SellQuantity += item.Quantity;
                        machineStock.Mender = operater;
                        //machineStock.MendTime = this.DateTime;

                        var storeSellStockLog = new StoreSellChannelStockLog();
                        storeSellStockLog.Id = GuidUtil.New();
                        //storeSellStockLog.MerchantId = item.MerchantId;
                        storeSellStockLog.StoreId = item.StoreId;
                        //storeSellStockLog.ChannelType = item.ChannelType;
                        //storeSellStockLog.ChannelId = item.ChannelId;
                        storeSellStockLog.SlotId = item.SlotId;
                        storeSellStockLog.ProductSkuId = item.ProductSkuId;
                        //storeSellStockLog.Quantity = machineStock.Quantity;
                        storeSellStockLog.LockQuantity = machineStock.LockQuantity;
                        storeSellStockLog.SellQuantity = machineStock.SellQuantity;
                        //storeSellStockLog.ChangeType = Enumeration.MachineStockLogChangeTpye.Lock;
                        storeSellStockLog.ChangeQuantity = item.Quantity;
                        storeSellStockLog.Creator = operater;
                        //storeSellStockLog.CreateTime = this.DateTime;
                        storeSellStockLog.RemarkByDev = string.Format("取消订单，恢复库存：{0}", item.Quantity);
                        //CurrentDb.StoreSellChannelStock.Add(storeSellStockLog);
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "操作成功");
                }
            }

            return result;
        }
    }
}
