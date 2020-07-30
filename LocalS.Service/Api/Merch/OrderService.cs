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
                         select new { o.Id, o.SellChannelRefIds, o.StoreId, o.ClientUserId, o.ClientUserName, o.StoreName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

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
                //query = query.Where(m => m.ExIsHappen == true && m.ExIsHandle == false);
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

                List<object> receiveDetails = new List<object>();
                foreach (var orderDetail in orderSub)
                {
                    List<object> pickupSkus = new List<object>();

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

                    receiveDetails.Add(new
                    {
                        Name = orderDetail.ReceiveModeName,
                        Mode = orderDetail.ReceiveMode,
                        DetailType = 1,
                        DetailItems = pickupSkus
                    });


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
                   // ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
//CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    ReceiveDetails = receiveDetails
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
            //ret.CanHandleEx = BizFactory.Order.GetCanHandleEx(order.ExIsHappen, order.ExIsHandle);
            //ret.ExHandleRemark = order.ExHandleRemark;
            //ret.ExIsHappen = order.ExIsHappen;
            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).ToList();

            List<RetOrderDetails.ReceiveDetail> receiveDetails = new List<RetOrderDetails.ReceiveDetail>();
            foreach (var orderSub in orderSubs)
            {
                var receiveDetail = new RetOrderDetails.ReceiveDetail();

                receiveDetail.Mode = orderSub.ReceiveMode;
                receiveDetail.Name = orderSub.ReceiveModeName;
                receiveDetail.DetailType = 1;

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

                    receiveDetail.DetailItems.Add(new RetOrderDetails.PickupSku
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

                ret.ReceiveDetails.Add(receiveDetail);

            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;

        }

    }
}
