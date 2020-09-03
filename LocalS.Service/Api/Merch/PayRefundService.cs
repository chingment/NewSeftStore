using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.BLL.Task;
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
        public StatusModel GetStatus(E_PayRefundStatus status)
        {
            var model = new StatusModel();

            switch (status)
            {
                case E_PayRefundStatus.WaitHandle:
                    model.Value = 1;
                    model.Text = "待处理";
                    break;
                case E_PayRefundStatus.Handling:
                    model.Value = 2;
                    model.Text = "处理中";
                    break;
                case E_PayRefundStatus.Success:
                    model.Value = 3;
                    model.Text = "成功";
                    break;
                case E_PayRefundStatus.Failure:
                    model.Value = 4;
                    model.Text = "失败";
                    break;
                case E_PayRefundStatus.InVaild:
                    model.Value = 5;
                    model.Text = "无效";
                    break;
            }
            return model;
        }

        public StatusModel GetMethod(E_PayRefundMethod method)
        {
            var model = new StatusModel();

            switch (method)
            {
                case E_PayRefundMethod.Original:
                    model.Value = 1;
                    model.Text = "原路退回";
                    break;
                case E_PayRefundMethod.Manual:
                    model.Value = 2;
                    model.Text = "线下处理";
                    break;
            }
            return model;
        }


        public CustomJsonResult GetList(string operater, string merchId, RupPayRefundGetList rup)
        {
            var result = new CustomJsonResult();


            var query = (from o in CurrentDb.PayRefund
                         where
                              (rup.PayRefundId == null || o.Id.Contains(rup.PayRefundId)) &&
                            (rup.OrderId == null || o.OrderId.Contains(rup.OrderId)) &&
                            (rup.PayTransId == null || o.Id.Contains(rup.PayTransId)) &&
                            (rup.PayPartnerOrderId == null || o.PayPartnerPayTransId.Contains(rup.PayPartnerOrderId)) &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.StoreName, o.ApplyMethod, o.OrderId, o.Status, o.ApplyAmount, o.PayPartnerPayTransId, o.ApplyRemark, o.PayTransId, o.ApplyTime, o.CreateTime });

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
                    StoreId = item.StoreId,
                    StoreName = item.StoreName,
                    OrderId = item.OrderId,
                    ApplyAmount = item.ApplyAmount,
                    ApplyMethod = GetMethod(item.ApplyMethod),
                    Status = GetStatus(item.Status),
                    PayPartnerPayTransId = item.PayPartnerPayTransId,
                    ApplyRemark = item.ApplyRemark,
                    PayTransId = item.PayTransId,
                    ApplyTime = item.ApplyTime.ToUnifiedFormatDateTime(),
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }
        public CustomJsonResult SearchOrder(string operater, string merchId, RupPayRefundSearchOrder rup)
        {
            var result = new CustomJsonResult();


            var query = (from o in CurrentDb.Order
                         where
                         ((rup.OrderId != null && o.Id == rup.OrderId) ||
                         (rup.PayTransId != null && o.PayTransId == rup.PayTransId) ||
                         (rup.PayPartnerOrderId != null && o.PayPartnerPayTransId == rup.PayPartnerOrderId)) &&
                         o.PayStatus == E_PayStatus.PaySuccess &&
                         o.MerchId == merchId
                         select new { o.Id, o.StoreId, o.StoreName, o.ReceiveMode, o.PayTransId, o.Status, o.PickupIsTrg, o.ExIsHappen, o.ExIsHandle, o.ReceiveModeName, o.SellChannelRefId, o.SellChannelRefType, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.Quantity, o.AppId, o.IsTestMode, o.ClientUserId, o.SubmittedTime, o.ClientUserName, o.Source, o.PayedTime, o.PayWay, o.CreateTime, o.PayStatus, o.PayPartnerPayTransId });

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
                    PayTransId = item.PayTransId,
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
                    PayPartnerPayTransId = item.PayPartnerPayTransId,
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
            ret.SubmittedTime = order.SubmittedTime.ToUnifiedFormatDateTime();
            ret.ChargeAmount = order.ChargeAmount.ToF2Price();
            ret.DiscountAmount = order.DiscountAmount.ToF2Price();
            ret.OriginalAmount = order.OriginalAmount.ToF2Price();
            ret.Quantity = order.Quantity;
            ret.Status = BizFactory.Order.GetStatus(order.Status);
            ret.SourceName = BizFactory.Order.GetSourceName(order.Source);
            ret.CanHandleEx = BizFactory.Order.GetCanHandleEx(order.ExIsHappen, order.ExIsHandle);
            ret.ExHandleRemark = order.ExHandleRemark;
            ret.ExIsHappen = order.ExIsHappen;


            var payRefund = CurrentDb.PayRefund.Where(m => m.OrderId == orderId).ToList();

            decimal refundedAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
            decimal refundingAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Handling).Sum(m => m.ApplyAmount);
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

            if (rop.Amount <= 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "退款金额必须大于0且不能大于可退金额");
            }

            if (rop.Method == E_PayRefundMethod.Unknow)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择退款方式");
            }

            if (string.IsNullOrEmpty(rop.Remark))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "退款原因不能为空");
            }

            var order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == rop.OrderId).FirstOrDefault();
            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到订单信息");
            }

            if (order.ExIsHappen && !order.ExIsHandle)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单发生异常没有处理，请到订单处理，再进行退款操作");
            }


            var payRefunds = CurrentDb.PayRefund.Where(m => m.OrderId == rop.OrderId).ToList();

            decimal refundedAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
            decimal refundingAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Sum(m => m.ApplyAmount);

            if (rop.Amount > (order.ChargeAmount - (refundedAmount + refundingAmount)))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "退款的金额不能大于可退金额");
            }

            using (TransactionScope ts = new TransactionScope())
            {
                var payTran = CurrentDb.PayTrans.Where(m => m.Id == order.PayTransId).FirstOrDefault();

                if (rop.Amount > payTran.ChargeAmount)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "退款的金额不能大于可退金额");
                }


                string payRefundId = IdWorker.Build(IdType.PayRefundId);

                var payRefund = new PayRefund();
                payRefund.Id = payRefundId;
                payRefund.MerchId = order.MerchId;
                payRefund.MerchName = order.MerchName;
                payRefund.StoreId = order.StoreId;
                payRefund.StoreName = order.StoreName;
                payRefund.ClientUserId = order.ClientUserId;
                payRefund.ClientUserName = order.ClientUserName;
                payRefund.OrderId = order.Id;
                payRefund.PayPartnerPayTransId = order.PayPartnerPayTransId;
                payRefund.PayTransId = order.PayTransId;
                payRefund.ApplyTime = DateTime.Now;
                payRefund.ApplyMethod = rop.Method;
                payRefund.ApplyRemark = rop.Remark;
                payRefund.ApplyAmount = rop.Amount;
                payRefund.Applyer = operater;
                payRefund.Status = E_PayRefundStatus.WaitHandle;
                payRefund.CreateTime = DateTime.Now;
                payRefund.Creator = operater;

                CurrentDb.PayRefund.Add(payRefund);
                CurrentDb.SaveChanges();
                ts.Complete();

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "提交成功", new { PayRefundId = payRefund.Id });

            }

            return result;

        }

        public CustomJsonResult GetListByHandle(string operater, string merchId, RupPayRefundGetList rup)
        {
            var result = new CustomJsonResult();


            var query = (from o in CurrentDb.PayRefund
                         where
                              (rup.PayRefundId == null || o.Id.Contains(rup.PayRefundId)) &&
                            (rup.OrderId == null || o.OrderId.Contains(rup.OrderId)) &&
                            (rup.PayTransId == null || o.Id.Contains(rup.PayTransId)) &&
                            (rup.PayPartnerOrderId == null || o.PayPartnerPayTransId.Contains(rup.PayPartnerOrderId)) &&
                         o.MerchId == merchId &&
                         (o.Status == E_PayRefundStatus.WaitHandle || o.Status == E_PayRefundStatus.Handling)
                         select new { o.Id, o.StoreId, o.StoreName, o.ApplyMethod, o.OrderId, o.Status, o.ApplyAmount, o.PayPartnerPayTransId, o.ApplyRemark, o.PayTransId, o.ApplyTime, o.CreateTime });

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
                    StoreId = item.StoreId,
                    StoreName = item.StoreName,
                    OrderId = item.OrderId,
                    ApplyAmount = item.ApplyAmount,
                    ApplyMethod = GetMethod(item.ApplyMethod),
                    Status = GetStatus(item.Status),
                    PayPartnerPayTransId = item.PayPartnerPayTransId,
                    ApplyRemark = item.ApplyRemark,
                    PayTransId = item.PayTransId,
                    ApplyTime = item.ApplyTime.ToUnifiedFormatDateTime(),
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }


        public CustomJsonResult GetHandleDetails(string operater, string merchId, string payRefundId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPayRefundHandleDetails();


            var payRefund = CurrentDb.PayRefund.Where(m => m.Id == payRefundId).FirstOrDefault();

            ret.PayRefundId = payRefundId;
            ret.ApplyAmount = payRefund.ApplyAmount.ToF2Price();
            ret.ApplyMethod = GetMethod(payRefund.ApplyMethod);
            ret.ApplyTime = payRefund.ApplyTime.ToUnifiedFormatDateTime();
            ret.ApplyRemark = payRefund.ApplyRemark;

            var order = CurrentDb.Order.Where(m => m.Id == payRefund.OrderId).FirstOrDefault();


            ret.Order.Id = order.Id;
            ret.Order.ClientUserName = order.ClientUserName;
            ret.Order.ClientUserId = order.ClientUserId;
            ret.Order.StoreName = order.StoreName;
            ret.Order.SubmittedTime = order.SubmittedTime.ToUnifiedFormatDateTime();
            ret.Order.ChargeAmount = order.ChargeAmount.ToF2Price();
            ret.Order.DiscountAmount = order.DiscountAmount.ToF2Price();
            ret.Order.OriginalAmount = order.OriginalAmount.ToF2Price();
            ret.Order.Quantity = order.Quantity;
            ret.Order.Status = BizFactory.Order.GetStatus(order.Status);
            ret.Order.SourceName = BizFactory.Order.GetSourceName(order.Source);

            var payRefunds = CurrentDb.PayRefund.Where(m => m.OrderId == order.Id).ToList();

            decimal refundedAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
            decimal refundingAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Sum(m => m.ApplyAmount);
            ret.Order.RefundedAmount = refundedAmount.ToF2Price();
            ret.Order.RefundingAmount = refundingAmount.ToF2Price();
            ret.Order.RefundableAmount = (order.ChargeAmount - refundedAmount - refundingAmount).ToF2Price();

            var receiveMode = new RetPayRefundHandleDetails.ReceiveMode();
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

            ret.Order.ReceiveModes.Add(receiveMode);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;

        }


        public CustomJsonResult Handle(string operater, string merchId, RopPayRefundHandle rop)
        {
            var result = new CustomJsonResult();


            using (TransactionScope ts = new TransactionScope())
            {

                if (rop.Result == RopPayRefundHandle.E_Result.Unknow)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "请选择处理结果");
                }

                if (string.IsNullOrEmpty(rop.Remark))
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "备注不能为空");
                }


                var payRefund = CurrentDb.PayRefund.Where(m => m.Id == rop.PayRefundId && m.Status == E_PayRefundStatus.WaitHandle).FirstOrDefault();

                if (payRefund == null)
                {
                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "找不到待处理信息");
                }


                if (payRefund.ApplyMethod == E_PayRefundMethod.Original)
                {
                    if (rop.Result == RopPayRefundHandle.E_Result.TurnToAutoRefund)
                    {
                        var order = CurrentDb.Order.Where(m => m.Id == payRefund.OrderId).FirstOrDefault();
                        var payTran = CurrentDb.PayTrans.Where(m => m.Id == payRefund.PayTransId).FirstOrDefault();

                        PayRefundResult payRefundResult = null;
                        switch (order.PayPartner)
                        {
                            case E_PayPartner.Wx:
                                var wxByNt_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(payTran.MerchId);
                                payRefundResult = SdkFactory.Wx.PayRefund(wxByNt_AppInfoConfig, order.PayTransId, rop.PayRefundId, payTran.ChargeAmount, rop.Amount, rop.Remark);
                                break;
                            case E_PayPartner.Xrt:
                                var xrt_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetXrtPayInfoConfg(payTran.MerchId);
                                payRefundResult = SdkFactory.XrtPay.PayRefund(xrt_AppInfoConfig, order.PayTransId, rop.PayRefundId, payTran.ChargeAmount, rop.Amount, rop.Remark);
                                break;

                        }

                        if (payRefundResult == null)
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "处理失败:" + payRefundResult.Message);
                        }

                        if (payRefundResult.Status != "APPLYING")
                        {
                            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "处理失败:" + payRefundResult.Message);
                        }

                        payRefund.Status = E_PayRefundStatus.Handling;
                        payRefund.MendTime = DateTime.Now;
                        payRefund.Mender = operater;

                        CurrentDb.SaveChanges();
                        ts.Complete();


                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "转入自动退款成功", new { PayRefundId = payRefund.Id });

                        Task4Factory.Tim2Global.Enter(Task4TimType.PayRefundCheckStatus, rop.PayRefundId, DateTime.Now.AddDays(3), new PayRefund2CheckStatusModel { Id = rop.PayRefundId, MerchId = order.MerchId, PayTransId = order.PayTransId, PayPartner = order.PayPartner });

                    }
                    else if (rop.Result == RopPayRefundHandle.E_Result.InVaild)
                    {
                        //payRefund.Status = E_PayRefundStatus.InVaild;
                        //payRefund.MendTime = DateTime.Now;
                        //payRefund.Mender = operater;

                        result = BizFactory.Order.PayRefundHandle(operater, rop.PayRefundId, "INVAILD", rop.Amount, rop.Remark);

                        CurrentDb.SaveChanges();
                        ts.Complete();
                    }
                }
                else if (payRefund.ApplyMethod == E_PayRefundMethod.Manual)
                {

                    string refundStatus = "";
                    if (rop.Result == RopPayRefundHandle.E_Result.Success)
                    {
                        refundStatus = "SUCCESS";
                    }
                    else if (rop.Result == RopPayRefundHandle.E_Result.Failure)
                    {
                        refundStatus = "FAIL";
                    }
                    else if (rop.Result == RopPayRefundHandle.E_Result.InVaild)
                    {
                        refundStatus = "INVAILD";
                    }

                    result = BizFactory.Order.PayRefundHandle(operater, rop.PayRefundId, refundStatus, rop.Amount, rop.Remark);
                    CurrentDb.SaveChanges();
                    ts.Complete();
                }
            }

            return result;

        }
    }
}