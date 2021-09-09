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
    public class OrderService : BaseService
    {
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
            ret.SubmittedTime = order.SubmittedTime.ToUnifiedFormatDateTime();
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
            ret.IsTestMode = order.IsTestMode;
            ret.ReceiveMode = order.ReceiveMode;
            ret.DeviceCumCode = MerchServiceFactory.Device.GetCode(order.DeviceId, order.DeviceCumCode);
            ret.RefundedAmount = order.RefundedAmount;
            ret.RefundedQuantity = order.RefundedQuantity;
            ret.PayWay = BizFactory.Order.GetPayWay(order.PayWay);

            var d_PayRefunds = CurrentDb.PayRefund.Where(m => m.OrderId == order.Id).ToList();

            foreach (var d_PayRefund in d_PayRefunds)
            {
                decimal amount = d_PayRefund.ApplyAmount;
                string dateTime = d_PayRefund.ApplyTime.ToUnifiedFormatDateTime();

                if (d_PayRefund.Status == E_PayRefundStatus.Success)
                {
                    amount = d_PayRefund.RefundedAmount;
                    dateTime = d_PayRefund.RefundedTime.ToUnifiedFormatDateTime();
                }
                else if (d_PayRefund.Status == E_PayRefundStatus.Failure)
                {
                    dateTime = d_PayRefund.HandleTime.ToUnifiedFormatDateTime();
                }

                ret.RefundRecords.Add(new { Id = d_PayRefund.Id, Amount = amount, Status = MerchServiceFactory.PayRefund.GetStatus(d_PayRefund.Status), DateTime = dateTime });
            }

            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).OrderByDescending(m => m.PickupStartTime).ToList();

            foreach (var orderSub in orderSubs)
            {
                var orderPickupLogs = CurrentDb.OrderPickupLog.Where(m => m.UniqueId == orderSub.Id).OrderByDescending(m => m.MsgId).ToList();

                List<object> pickupLogs = new List<object>();

                foreach (var orderPickupLog in orderPickupLogs)
                {
                    string imgUrl = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId);
                    string imgUrl2 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId2);
                    string imgUrl3 = BizFactory.Order.GetPickImgUrl(orderPickupLog.ImgId3);
                    List<string> imgUrls = new List<string>();
                    if (!string.IsNullOrEmpty(imgUrl))
                    {
                        imgUrls.Add(imgUrl);
                    }
                    if (!string.IsNullOrEmpty(imgUrl2))
                    {
                        imgUrls.Add(imgUrl2);
                    }
                    if (!string.IsNullOrEmpty(imgUrl3))
                    {
                        imgUrls.Add(imgUrl3);
                    }
                    pickupLogs.Add(new { Timestamp = orderPickupLog.CreateTime.ToUnifiedFormatDateTime(), Content = orderPickupLog.ActionRemark, ImgUrl = imgUrl, ImgUrls = imgUrls });
                }

                ret.Skus.Add(new
                {
                    UniqueId = orderSub.Id,
                    MainImgUrl = orderSub.SkuMainImgUrl,
                    Name = orderSub.SkuName,
                    Quantity = orderSub.Quantity,
                    ExPickupIsHandle = orderSub.ExPickupIsHandle,
                    Status = BizFactory.Order.GetPickupStatus(orderSub.PickupStatus),
                    PickupLogs = pickupLogs,
                    SignStatus = 0
                });
            }


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;

        }

        public CustomJsonResult GetList(string operater, string merchId, RupOrderGetList rup)
        {
            var result = new CustomJsonResult();


            var query = (from o in CurrentDb.Order
                         where (rup.ClientUserName == null || o.ClientUserName.Contains(rup.ClientUserName))
                         &&
                         o.PayStatus == E_PayStatus.PaySuccess
                         &&
                         (rup.OrderId == null || o.Id.Contains(rup.OrderId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.IsTestMode, o.DeviceCumCode, o.DeviceId, o.StoreName, o.PickupIsTrg, o.ReceiveModeName, o.ReceiveMode, o.ExIsHappen, o.ClientUserId, o.ExIsHandle, o.ClientUserName, o.Source, o.SubmittedTime, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.CreateTime, o.Quantity, o.Status });

            if (rup.OrderStatus != Entity.E_OrderStatus.Unknow)
            {
                query = query.Where(m => m.Status == rup.OrderStatus);
            }

            if (!string.IsNullOrEmpty(rup.StoreId))
            {
                query = query.Where(m => m.StoreId == rup.StoreId);
            }

            if (!string.IsNullOrEmpty(rup.ClientUserId))
            {
                query = query.Where(m => m.ClientUserId == rup.ClientUserId);
            }

            if (rup.IsHasEx)
            {
                query = query.Where(m => m.ExIsHappen == true && m.ExIsHandle == false);
            }

            if (rup.PickupTrgStatus == 1)
            {
                query = query.Where(m => m.PickupIsTrg == false);
            }
            else if (rup.PickupTrgStatus == 2)
            {
                query = query.Where(m => m.PickupIsTrg == true);
            }

            if (rup.ReceiveMode != E_ReceiveMode.Unknow)
            {
                query = query.Where(m => m.ReceiveMode == rup.ReceiveMode);
            }

            if (!string.IsNullOrEmpty(rup.DeviceId))
            {
                query = query.Where(m => m.DeviceId == rup.DeviceId);
            }

            if (!string.IsNullOrEmpty(rup.DeviceCumCode))
            {
                query = query.Where(m => m.DeviceCumCode == rup.DeviceCumCode || m.DeviceId == rup.DeviceCumCode);
            }

            if (rup.SubmittedTimeArea != null)
            {
                if (rup.SubmittedTimeArea.Length == 2)
                {
                    DateTime? submittedStartTime = DateTime.Parse(rup.SubmittedTimeArea[0]);
                    DateTime? submittedEndTime = DateTime.Parse(rup.SubmittedTimeArea[1]);

                    if (submittedStartTime != null && submittedEndTime != null)
                    {
                        query = query.Where(m => m.SubmittedTime >= submittedStartTime && m.SubmittedTime <= submittedEndTime);

                    }
                }
            }

            int total = query.Count();

            int pageIndex = rup.Page - 1;
            int pageSize = rup.Limit;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    ClientUserName = item.ClientUserName,
                    ClientUserId = item.ClientUserId,
                    StoreName = item.StoreName,
                    DeviceCode = MerchServiceFactory.Device.GetCode(item.DeviceId, item.DeviceCumCode),
                    SubmittedTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    Status = BizFactory.Order.GetStatus(item.Status),
                    SourceName = BizFactory.Order.GetSourceName(item.Source),
                    ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    ReceiveMode = item.ReceiveMode,
                    ReceiveModeName = item.ReceiveModeName,
                    PickupTrgStatus = BizFactory.Order.GetPickupTrgStatus(item.ReceiveMode, item.PickupIsTrg),
                    IsTestMode = item.IsTestMode
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult HandleExByDeviceSelfTake(string operater, string merchId, RopOrderHandleExByDeviceSelfTake rop)
        {
            var bizRop = new BLL.Biz.RopOrderHandleExByDeviceSelfTake();
            bizRop.AppId = rop.AppId;
            bizRop.MerchId = merchId;
            bizRop.DeviceId = rop.DeviceId;
            bizRop.IsRunning = rop.IsRunning;
            bizRop.Remark = rop.Remark;
            bizRop.Items.Add(new ExItem { ItemId = rop.Id, Uniques = rop.Uniques });
            var result = BizFactory.Order.HandleExByDeviceSelfTake(operater, bizRop);
            return result;
        }

        public CustomJsonResult SendDeviceShip(string operater, string merchId, RopOrderHandleExByDeviceSelfTake rop)
        {
            var result = BizFactory.Order.SendDeviceShip(operater, merchId, rop.Id);
            return result;
        }
    }
}
