using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.Merch
{
    public class OrderService : BaseDbContext
    {

        public CustomJsonResult GetList(string operater, string merchId, RupOrderGetList rup)
        {
            var result = new CustomJsonResult();

            var query = (from o in CurrentDb.Order
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         (rup.OrderId == null || o.Id.Contains(rup.OrderId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.SellChannelRefIds, o.StoreId, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.StoreName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

            if (rup.OrderStauts != Entity.E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.OrderStauts);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
            }

            if (!string.IsNullOrEmpty(rup.MachineId))
            {
                query = query.Where(m => m.SellChannelRefIds.Contains(rup.MachineId));
            }

            if (!string.IsNullOrEmpty(rup.ClientUserId))
            {
                query = query.Where(m => m.ClientUserId == rup.ClientUserId);
            }

            if (rup.IsHasEx)
            {
                query = query.Where(m => m.ExIsHappen == true && m.ExIsHandle == false);
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == item.Id).ToList();

                List<object> sellChannelDetails = new List<object>();
                foreach (var orderDetail in orderSub)
                {
                    List<object> pickupSkus = new List<object>();
                    switch (orderDetail.SellChannelRefType)
                    {
                        case E_SellChannelRefType.Machine:

                            var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == item.Id).OrderByDescending(m => m.PickupStartTime).ToList();


                            foreach (var orderSubChild in orderSubChilds)
                            {
                                var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChild.Id).OrderByDescending(m => m.CreateTime).ToList();

                                List<object> pickupLogs = new List<object>();

                                foreach (var orderPickupLog in orderPickupLogs)
                                {
                                    string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                                    string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                                    List<string> imgUrls = new List<string>();
                                    if (!string.IsNullOrEmpty(imgUrl))
                                    {
                                        imgUrls.Add(imgUrl);
                                    }

                                    if (!string.IsNullOrEmpty(imgUrl2))
                                    {
                                        imgUrls.Add(imgUrl2);
                                    }

                                    pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                                }

                                pickupSkus.Add(new
                                {
                                    Id = orderSubChild.PrdProductSkuId,
                                    MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl,
                                    UniqueId = orderSubChild.Id,
                                    ExPickupIsHandle = orderSubChild.ExPickupIsHandle,
                                    Name = orderSubChild.PrdProductSkuName,
                                    Quantity = orderSubChild.Quantity,
                                    Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus),
                                    PickupLogs = pickupLogs
                                });
                            }

                            sellChannelDetails.Add(new
                            {
                                Name = orderDetail.SellChannelRefName,
                                Type = orderDetail.SellChannelRefType,
                                DetailType = 1,
                                DetailItems = pickupSkus
                            });

                            break;
                    }
                }

                olist.Add(new
                {
                    Id = item.Id,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    SubmitTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = BizFactory.Order.GetStatus(item.Status),
                    SourceName = BizFactory.Order.GetSourceName(item.Source),
                    ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    SellChannelDetails = sellChannelDetails
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetDetails(string operater, string merchId, string orderId)
        {
            var result = new CustomJsonResult();

            var ret = new RetOrderDetails();

            var order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == orderId).FirstOrDefault();
            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            ret.Id = order.Id;
            ret.ClientUserName = order.ClientUserName;
            ret.ClientUserId = order.ClientUserId;
            ret.StoreName = order.StoreName;
            ret.SubmitTime = order.SubmittedTime.ToUnifiedFormatDateTime();
            ret.ChargeAmount = order.ChargeAmount.ToF2Price();
            ret.DiscountAmount = order.DiscountAmount.ToF2Price();
            ret.OriginalAmount = order.OriginalAmount.ToF2Price();
            ret.Quantity = order.Quantity;
            ret.CreateTime = order.CreateTime.ToUnifiedFormatDateTime();
            ret.Status = BizFactory.Order.GetStatus(order.Status);
            ret.SourceName = BizFactory.Order.GetSourceName(order.Source);
            ret.CanHandleEx = BizFactory.Order.GetCanHandleEx(order.ExIsHappen, order.ExIsHandle);
            ret.ExHandleRemark = order.ExHandleRemark;
            ret.ExIsHappen = order.ExIsHappen;
            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).ToList();

            List<RetOrderDetails.SellChannelDetail> sellChannelDetails = new List<RetOrderDetails.SellChannelDetail>();
            foreach (var orderSub in orderSubs)
            {
                var sellChannelDetail = new RetOrderDetails.SellChannelDetail();

                switch (orderSub.SellChannelRefType)
                {
                    case E_SellChannelRefType.Machine:
                        sellChannelDetail.Type = E_SellChannelRefType.Machine;
                        sellChannelDetail.Name = orderSub.SellChannelRefName;
                        sellChannelDetail.DetailType = 1;

                        var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == orderSub.Id).OrderByDescending(m => m.PickupStartTime).ToList();
                        var pickupSkus = new List<RetOrderDetails.PickupSku>();
                        foreach (var orderSubChild in orderSubChilds)
                        {
                            var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSubChild.Id).OrderByDescending(m => m.CreateTime).ToList();

                            List<RetOrderDetails.PickupLog> pickupLogs = new List<RetOrderDetails.PickupLog>();

                            foreach (var orderPickupLog in orderPickupLogs)
                            {
                                string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                                string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                                List<string> imgUrls = new List<string>();
                                if (!string.IsNullOrEmpty(imgUrl))
                                {
                                    imgUrls.Add(imgUrl);
                                }
                                if (!string.IsNullOrEmpty(imgUrl2))
                                {
                                    imgUrls.Add(imgUrl2);
                                }
                                pickupLogs.Add(new RetOrderDetails.PickupLog { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                            }

                            sellChannelDetail.DetailItems.Add(new RetOrderDetails.PickupSku
                            {
                                Id = orderSubChild.PrdProductSkuId,
                                ExPickupIsHandle = orderSubChild.ExPickupIsHandle,
                                UniqueId = orderSubChild.Id,
                                MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl,
                                Name = orderSubChild.PrdProductSkuName,
                                Quantity = orderSubChild.Quantity,
                                Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus),
                                PickupLogs = pickupLogs,
                                SignStatus = 0
                            });
                        }

                        ret.SellChannelDetails.Add(sellChannelDetail);
                        break;
                }
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;

        }

        public CustomJsonResult HandleExOrder(string operater, string merchId, RopOrderHandleExOrder rop)
        {
            var result = new CustomJsonResult();

            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(rop.Id))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "订单ID不能为空");
                }

                if (rop.UniqueItems.Count == 0)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单要处理的信息为空");
                }

                var order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == rop.Id).FirstOrDefault();
                if (order == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单信息不存在");
                }

                if (!order.ExIsHappen)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不是异常状态，不能处理");
                }

                if (order.ExIsHandle)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该异常订单已经处理完毕");
                }


                var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == rop.Id && m.ExIsHappen == true && m.ExIsHandle == false).ToList();

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderId == rop.Id && m.ExPickupIsHappen == true && m.ExPickupIsHandle == false && m.PickupStatus == E_OrderPickupStatus.Exception).ToList();

                foreach (var orderSubChild in orderSubChilds)
                {
                    var detailItem = rop.UniqueItems.Where(m => m.UniqueId == orderSubChild.Id).FirstOrDefault();
                    if (detailItem == null)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单里对应商品异常记录未找到");
                    }

                    if (detailItem.SignStatus != 1 && detailItem.SignStatus != 2)
                    {
                        return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单不能处理该异常状态:" + detailItem.SignStatus);
                    }

                    if (detailItem.SignStatus == 1)
                    {

                        if (orderSubChild.PickupStatus != E_OrderPickupStatus.Taked && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                        {
                            BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPickupOneManMadeSignTakeByNotComplete, AppId.MERCH, orderSubChild.MerchId, orderSubChild.StoreId, orderSubChild.SellChannelRefId, orderSubChild.CabinetId, orderSubChild.SlotId, orderSubChild.PrdProductSkuId, 1);
                        }

                        orderSubChild.ExPickupIsHandle = true;
                        orderSubChild.ExPickupHandleTime = DateTime.Now;
                        orderSubChild.ExPickupHandleSign = E_OrderExPickupHandleSign.Taked;
                        orderSubChild.PickupStatus = E_OrderPickupStatus.ExPickupSignTaked;


                        var orderPickupLog = new OrderPickupLog();
                        orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                        orderPickupLog.OrderId = orderSubChild.OrderId;
                        orderPickupLog.OrderSubId = orderSubChild.OrderSubId;
                        orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                        orderPickupLog.SellChannelRefId = orderSubChild.SellChannelRefId;
                        orderPickupLog.UniqueId = orderSubChild.Id;
                        orderPickupLog.PrdProductSkuId = orderSubChild.PrdProductSkuId;
                        orderPickupLog.CabinetId = orderSubChild.CabinetId;
                        orderPickupLog.SlotId = orderSubChild.SlotId;
                        orderPickupLog.Status = E_OrderPickupStatus.ExPickupSignTaked;
                        orderPickupLog.IsPickupComplete = true;
                        orderPickupLog.ActionRemark = "人为标识已取货";
                        orderPickupLog.Remark = "";
                        orderPickupLog.CreateTime = DateTime.Now;
                        orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(orderPickupLog);
                    }
                    else if (detailItem.SignStatus == 2)
                    {
                        if (orderSubChild.PickupStatus != E_OrderPickupStatus.Taked && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && orderSubChild.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                        {
                            BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.StockOrderPickupOneManMadeSignNotTakeByNotComplete, AppId.STORETERM, orderSubChild.MerchId, orderSubChild.StoreId, orderSubChild.SellChannelRefId, orderSubChild.CabinetId, orderSubChild.SlotId, orderSubChild.PrdProductSkuId, 1);
                        }

                        orderSubChild.ExPickupIsHandle = true;
                        orderSubChild.ExPickupHandleTime = DateTime.Now;
                        orderSubChild.ExPickupHandleSign = E_OrderExPickupHandleSign.UnTaked;
                        orderSubChild.PickupStatus = E_OrderPickupStatus.ExPickupSignUnTaked;

                        var orderPickupLog = new OrderPickupLog();
                        orderPickupLog.Id = IdWorker.Build(IdType.NewGuid);
                        orderPickupLog.OrderId = orderSubChild.OrderId;
                        orderPickupLog.OrderSubId = orderSubChild.OrderSubId;
                        orderPickupLog.SellChannelRefType = E_SellChannelRefType.Machine;
                        orderPickupLog.SellChannelRefId = orderSubChild.SellChannelRefId;
                        orderPickupLog.UniqueId = orderSubChild.Id;
                        orderPickupLog.PrdProductSkuId = orderSubChild.PrdProductSkuId;
                        orderPickupLog.CabinetId = orderSubChild.CabinetId;
                        orderPickupLog.SlotId = orderSubChild.SlotId;
                        orderPickupLog.Status = E_OrderPickupStatus.ExPickupSignUnTaked;
                        orderPickupLog.IsPickupComplete = false;
                        orderPickupLog.ActionRemark = "人为标识未取货";
                        orderPickupLog.Remark = "";
                        orderPickupLog.CreateTime = DateTime.Now;
                        orderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(orderPickupLog);
                    }
                }

                foreach (var orderSub in orderSubs)
                {
                    orderSub.ExIsHandle = true;
                    orderSub.ExHandleTime = DateTime.Now;
                }

                order.ExIsHandle = true;
                order.ExHandleTime = DateTime.Now;
                order.ExHandleRemark = rop.Remark;
                order.Status = E_OrderStatus.Completed;
                order.CompletedTime = DateTime.Now;


                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, "", "", EventCode.OrderHandleExOrder, string.Format("处理异常订单号：{0}，备注：{1}", order.Id, order.ExHandleRemark));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理成功");
            }

            return result;
        }


    }
}
