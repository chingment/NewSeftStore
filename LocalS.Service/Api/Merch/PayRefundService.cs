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
    public class PayRefundService : BaseDbContext
    {
        public CustomJsonResult SearchOrder(string operater, string merchId, RupPayRefundSearchOrder rup)
        {
            var result = new CustomJsonResult();


            var query = (from o in CurrentDb.Order
                         where
                         ((rup.OrderId != null && o.Id == rup.OrderId) ||
                         (rup.PayTransId != null && o.PayTransId == rup.PayTransId) ||
                         (rup.PayPartnerOrderId != null && o.PayPartnerOrderId == rup.PayPartnerOrderId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.StoreName, o.ReceiveMode, o.Status, o.PickupIsTrg, o.ExIsHappen, o.ExIsHandle, o.ReceiveModeName, o.SellChannelRefId, o.SellChannelRefType, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.Quantity, o.AppId, o.IsTestMode, o.ClientUserId, o.SubmittedTime, o.ClientUserName, o.Source, o.PayedTime, o.PayWay, o.CreateTime, o.PayStatus, o.PayPartnerOrderId });

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
                    ReceiveMode = item.ReceiveMode,
                    ReceiveModeName = item.ReceiveModeName,
                    ChargeAmount = item.ChargeAmount.ToF2Price(),
                    DiscountAmount = item.DiscountAmount.ToF2Price(),
                    OriginalAmount = item.OriginalAmount.ToF2Price(),
                    Quantity = item.Quantity,
                    CreateTime = item.CreateTime,
                    AppId = item.AppId,
                    IsTestMode = item.IsTestMode ? "是" : "否",
                    SubmittedTime = item.SubmittedTime.ToUnifiedFormatDateTime(),
                    PayStatus = BizFactory.Order.GetPayStatus(item.PayStatus),
                    PayedTime = item.PayedTime,
                    PayWay = BizFactory.Order.GetPayWay(item.PayWay),
                    PayPartnerOrderId = item.PayPartnerOrderId,
                    Status = BizFactory.Order.GetStatus(item.Status),
                    ExStatus = BizFactory.Order.GetExStatus(item.ExIsHappen, item.ExIsHandle),
                    CanHandleEx = BizFactory.Order.GetCanHandleEx(item.ExIsHappen, item.ExIsHandle),
                    PickupTrgStatus = BizFactory.Order.GetPickupTrgStatus(item.ReceiveMode, item.PickupIsTrg),
                    SourceName = BizFactory.Order.GetSourceName(item.Source),
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }


        public CustomJsonResult GetOrderDetails(string operater, string merchId, string orderId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPayRefundOrderDetails();

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


            var payRefund = CurrentDb.PayRefund.Where(m => m.OrderId == orderId).ToList();

            decimal refundedAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.Amount);
            decimal refundingAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Applying).Sum(m => m.Amount);
            ret.RefundedAmount = refundedAmount.ToF2Price();
            ret.RefundingAmount = refundingAmount.ToF2Price();
            ret.RefundableAmount = (order.ChargeAmount - refundedAmount - refundingAmount).ToF2Price();

            var receiveMode = new RetPayRefundOrderDetails.ReceiveMode();
            receiveMode.Mode = order.ReceiveMode;
            receiveMode.Name = order.ReceiveModeName;
            receiveMode.Type = 1;

            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).OrderByDescending(m => m.PickupStartTime).ToList();

            foreach (var orderSub in orderSubs)
            {
                receiveMode.Items.Add(new
                {
                    ExPickupIsHandle = orderSub.ExPickupIsHandle,
                    UniqueId = orderSub.Id,
                    MainImgUrl = orderSub.PrdProductSkuMainImgUrl,
                    Name = orderSub.PrdProductSkuName,
                    Quantity = orderSub.Quantity,
                    SalePrice = orderSub.SalePrice,
                    ChargeAmount = orderSub.ChargeAmount,
                    Status = BizFactory.Order.GetPickupStatus(orderSub.PickupStatus)
                });
            }

            ret.ReceiveModes.Add(receiveMode);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;

        }


        public CustomJsonResult Apply(string operater, string merchId, RopPayRefundApply rop)
        {
            var result = new CustomJsonResult();


            var order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == rop.OrderId).FirstOrDefault();
            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到订单信息");
            }

            if (rop.Amount > (order.ChargeAmount - order.RefundedAmount))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "退款的金额不能大于可退金额");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var payRefund = new PayRefund();

                payRefund.Id = IdWorker.Build(IdType.NewGuid);
                payRefund.MerchId = order.MerchId;
                payRefund.MerchName = order.MerchName;
                payRefund.StoreId = order.StoreId;
                payRefund.StoreName = order.StoreName;
                payRefund.ClientUserId = order.ClientUserId;
                payRefund.ClientUserName = order.ClientUserName;
                payRefund.OrderId = order.Id;
                payRefund.ApplyTime = DateTime.Now;
                payRefund.Method = rop.Method;
                payRefund.Reason = rop.Reason;
                payRefund.Amount = rop.Amount;
                payRefund.Operator = operater;
                payRefund.Status = E_PayRefundStatus.Applying;
                payRefund.CreateTime = DateTime.Now;
                payRefund.Creator = operater;

                CurrentDb.PayRefund.Add(payRefund);
                CurrentDb.SaveChanges();
                ts.Complete();


                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "申请成功");

            }

            return result;

        }
    }
}
