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
    public class PayRefundService : BaseService
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
                         select new { o.Id, o.StoreId, o.StoreName, o.ApplyMethod, o.OrderId, o.Status, o.ApplyAmount, o.PayPartnerPayTransId, o.ApplyRemark, o.PayTransId, o.ApplyTime, o.CreateTime, o.RefundedRemark, });

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
                    RefundedRemark = item.RefundedRemark
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

            return result;
        }

        public CustomJsonResult GetDetails(string operater, string merchId, string payRefundId)
        {
            var result = new CustomJsonResult();

            //var ret = new RetPayRefundApplyDetails();

            var d_PayRefund = CurrentDb.PayRefund.Where(m => m.MerchId == merchId && m.Id == payRefundId).FirstOrDefault();

            var d_Order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == d_PayRefund.OrderId).FirstOrDefault();


            //ret.Order.Id = order.Id;
            //ret.Order.ClientUserName = order.ClientUserName;
            //ret.Order.ClientUserId = order.ClientUserId;
            //ret.Order.StoreName = order.StoreName;
            //ret.Order.SubmittedTime = order.SubmittedTime.ToUnifiedFormatDateTime();
            //ret.Order.ChargeAmount = order.ChargeAmount.ToF2Price();
            //ret.Order.DiscountAmount = order.DiscountAmount.ToF2Price();
            //ret.Order.OriginalAmount = order.OriginalAmount.ToF2Price();
            //ret.Order.Quantity = order.Quantity;
            //ret.Order.Status = BizFactory.Order.GetStatus(order.Status);
            //ret.Order.SourceName = BizFactory.Order.GetSourceName(order.Source);
            //ret.Order.CanHandleEx = BizFactory.Order.GetCanHandleEx(order.ExIsHappen, order.ExIsHandle);
            //ret.Order.ExHandleRemark = order.ExHandleRemark;
            //ret.Order.ExIsHappen = order.ExIsHappen;
            //ret.Order.DeviceCumCode = MerchServiceFactory.Device.GetCode(order.DeviceId, order.DeviceCumCode);
            //ret.Order.PayWay = BizFactory.Order.GetPayWay(order.PayWay);

            //var payRefund = CurrentDb.PayRefund.Where(m => m.OrderId == orderId).ToList();

            //decimal refundedAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
            //decimal refundingAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Sum(m => m.ApplyAmount);
            //ret.Order.RefundedAmount = refundedAmount.ToF2Price();
            //ret.Order.RefundingAmount = refundingAmount.ToF2Price();
            //ret.Order.RefundableAmount = (order.ChargeAmount - refundedAmount - refundingAmount).ToF2Price();


            //var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).OrderByDescending(m => m.PickupStartTime).ToList();

            //foreach (var orderSub in orderSubs)
            //{
            //    ret.Order.Skus.Add(new
            //    {
            //        UniqueId = orderSub.Id,
            //        MainImgUrl = orderSub.SkuMainImgUrl,
            //        Name = orderSub.SkuName,
            //        Quantity = orderSub.Quantity,
            //        SalePrice = orderSub.SalePrice,
            //        ChargeAmount = orderSub.ChargeAmount,
            //        PickupStatus = BizFactory.Order.GetPickupStatus(orderSub.PickupStatus),
            //        RefundedAmount = orderSub.RefundedAmount,
            //        RefundedQuantity = orderSub.RefundedQuantity,
            //        ApplyRefundedQuantity = orderSub.Quantity,
            //        ApplyRefundedAmount = orderSub.ChargeAmount,
            //        ApplySignRefunded = false,
            //        ApplyCanSignRefunded = orderSub.IsRefunded == true ? false : true,
            //    });
            //}

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", null);
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
                         select new { o.Id, o.StoreId, o.StoreName, o.ReceiveMode, o.PayTransId, o.Status, o.PickupIsTrg, o.ExIsHappen, o.ExIsHandle, o.ReceiveModeName, o.DeviceId, o.ChargeAmount, o.DiscountAmount, o.OriginalAmount, o.Quantity, o.AppId, o.IsTestMode, o.ClientUserId, o.SubmittedTime, o.ClientUserName, o.Source, o.PayedTime, o.PayWay, o.CreateTime, o.PayStatus, o.PayPartnerPayTransId });

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

        public CustomJsonResult GetApplyDetails(string operater, string merchId, string orderId)
        {
            var result = new CustomJsonResult();

            var ret = new RetPayRefundApplyDetails();

            var order = CurrentDb.Order.Where(m => m.MerchId == merchId && m.Id == orderId).FirstOrDefault();
            if (order == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

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
            ret.Order.CanHandleEx = BizFactory.Order.GetCanHandleEx(order.ExIsHappen, order.ExIsHandle);
            ret.Order.ExHandleRemark = order.ExHandleRemark;
            ret.Order.ExIsHappen = order.ExIsHappen;
            ret.Order.DeviceCumCode = MerchServiceFactory.Device.GetCode(order.DeviceId, order.DeviceCumCode);
            ret.Order.PayWay = BizFactory.Order.GetPayWay(order.PayWay);

           var payRefund = CurrentDb.PayRefund.Where(m => m.OrderId == orderId).ToList();

            decimal refundedAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
            decimal refundingAmount = payRefund.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Sum(m => m.ApplyAmount);
            ret.Order.RefundedAmount = refundedAmount.ToF2Price();
            ret.Order.RefundingAmount = refundingAmount.ToF2Price();
            ret.Order.RefundableAmount = (order.ChargeAmount - refundedAmount - refundingAmount).ToF2Price();


            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).OrderByDescending(m => m.PickupStartTime).ToList();

            foreach (var orderSub in orderSubs)
            {
                ret.Order.Skus.Add(new
                {
                    UniqueId = orderSub.Id,
                    MainImgUrl = orderSub.SkuMainImgUrl,
                    Name = orderSub.SkuName,
                    Quantity = orderSub.Quantity,
                    SalePrice = orderSub.SalePrice,
                    ChargeAmount = orderSub.ChargeAmount,
                    PickupStatus = BizFactory.Order.GetPickupStatus(orderSub.PickupStatus),
                    RefundedAmount = orderSub.RefundedAmount,
                    RefundedQuantity = orderSub.RefundedQuantity,
                    ApplyRefundedQuantity = orderSub.Quantity,
                    ApplyRefundedAmount = orderSub.ChargeAmount,
                    ApplySignRefunded = false,
                    ApplyCanSignRefunded = orderSub.IsRefunded == true ? false : true,
                });
            }

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
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单信息查找不到");
            }

            if (order.ExIsHappen && !order.ExIsHandle)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单存在异常未处理，请到订单处理后，再进行申请退款操作");
            }


            var payRefunds = CurrentDb.PayRefund.Where(m => m.OrderId == rop.OrderId).ToList();

            var hasNoHandleCount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Count();

            if (hasNoHandleCount > 0)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该订单存在一笔退款未处理，请到退款处理后，再进行申请退款操作");
            }

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


                if (rop.RefundSkus != null)
                {
                    foreach (var refundSku in rop.RefundSkus)
                    {
                        var d_PayRefundSku = new PayRefundSku();
                        d_PayRefundSku.Id = IdWorker.Build(IdType.NewGuid);
                        d_PayRefundSku.PayRefundId = payRefundId;
                        d_PayRefundSku.UniqueId = refundSku.UniqueId;
                        d_PayRefundSku.ApplySignRefunded = true;
                        d_PayRefundSku.ApplyRefundedAmount = refundSku.RefundedAmount;
                        d_PayRefundSku.ApplyRefundedQuantity = refundSku.RefundedQuantity;
                        d_PayRefundSku.CreateTime = DateTime.Now;
                        d_PayRefundSku.Creator = operater;
                        CurrentDb.PayRefundSku.Add(d_PayRefundSku);
                    }
                }


                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.pay_refund_apply, string.Format("订单号:{0}，申请退款金额：{1}，提交成功，退款单号：{2}", payRefund.OrderId, payRefund.ApplyAmount.ToF2Price(), payRefund.Id), rop);

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
            ret.Order.DeviceCumCode = MerchServiceFactory.Device.GetCode(order.DeviceId, order.DeviceCumCode);
            ret.Order.PayWay = BizFactory.Order.GetPayWay(order.PayWay);

            var payRefunds = CurrentDb.PayRefund.Where(m => m.OrderId == order.Id).ToList();

            decimal refundedAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Success).Sum(m => m.ApplyAmount);
            decimal refundingAmount = payRefunds.Where(m => m.Status == E_PayRefundStatus.Handling || m.Status == E_PayRefundStatus.WaitHandle).Sum(m => m.ApplyAmount);
            ret.Order.RefundedAmount = refundedAmount.ToF2Price();
            ret.Order.RefundingAmount = refundingAmount.ToF2Price();
            ret.Order.RefundableAmount = (order.ChargeAmount - refundedAmount - refundingAmount).ToF2Price();

            var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == order.Id).OrderByDescending(m => m.PickupStartTime).ToList();

            foreach (var orderSub in orderSubs)
            {
                bool l_ApplySignRefunded = false;
                decimal l_ApplyRefundedAmount = 0m;
                int l_ApplyRefundedQuantity = 0;

                var d_PayRefundSku = CurrentDb.PayRefundSku.Where(m => m.PayRefundId == payRefund.Id && m.UniqueId == orderSub.Id).FirstOrDefault();
                if (d_PayRefundSku != null)
                {
                    l_ApplySignRefunded = true;
                    l_ApplyRefundedAmount = d_PayRefundSku.ApplyRefundedAmount;
                    l_ApplyRefundedQuantity = d_PayRefundSku.ApplyRefundedQuantity;
                }

                ret.Order.Skus.Add(new
                {
                    UniqueId = orderSub.Id,
                    MainImgUrl = orderSub.SkuMainImgUrl,
                    Name = orderSub.SkuName,
                    Quantity = orderSub.Quantity,
                    SalePrice = orderSub.SalePrice,
                    ChargeAmount = orderSub.ChargeAmount,
                    PickupStatus = BizFactory.Order.GetPickupStatus(orderSub.PickupStatus),
                    RefundedAmount = orderSub.RefundedAmount,
                    RefundedQuantity = orderSub.RefundedQuantity,
                    ApplySignRefunded = l_ApplySignRefunded,
                    ApplyRefundedAmount = l_ApplyRefundedAmount,
                    ApplyRefundedQuantity = l_ApplyRefundedQuantity
                });
            }


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

                string refundStatus = "";
                string refundRemark = "";
                decimal refundAmount = 0;
                DateTime refundTime = DateTime.Now;

                var order = CurrentDb.Order.Where(m => m.Id == payRefund.OrderId).FirstOrDefault();
                var payTran = CurrentDb.PayTrans.Where(m => m.Id == payRefund.PayTransId).FirstOrDefault();

                PayRefundResult api_Result = null;

                if (payRefund.ApplyMethod == E_PayRefundMethod.Original)
                {
                    #region  线上处理
                    if (rop.Result == RopPayRefundHandle.E_Result.TurnToAutoRefund)
                    {
                        switch (order.PayPartner)
                        {
                            case E_PayPartner.Wx:
                                #region Wx
                                var wx_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetWxMpAppInfoConfig(payTran.MerchId);
                                api_Result = SdkFactory.Wx.PayRefund(wx_AppInfoConfig, order.PayTransId, rop.PayRefundId, payTran.ChargeAmount, rop.Amount, rop.Remark);
                                if (api_Result == null)
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "处理失败:" + api_Result.Message);
                                }

                                if (api_Result.Status != "APPLYING")
                                {
                                    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "处理失败:" + api_Result.Message);
                                }

                                refundStatus = "HANDLING";
                                refundRemark = "退款中";

                                #endregion
                                break;
                            case E_PayPartner.Zfb:
                                #region 
                                var zfb_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetZfbMpAppInfoConfig(payTran.MerchId);
                                api_Result = SdkFactory.Zfb.PayRefund(zfb_AppInfoConfig, order.PayTransId, rop.PayRefundId, payTran.ChargeAmount, rop.Amount, rop.Remark);
                                if (api_Result.Status == "SUCCESS")
                                {
                                    refundStatus = "SUCCESS";
                                    refundTime = api_Result.RefundTime.Value;
                                    refundAmount = api_Result.RefundFee;
                                    refundRemark = "退款成功";
                                }
                                else
                                {
                                    refundStatus = "FAIL";
                                    refundRemark = "退款失败，" + api_Result.Message;
                                }
                                #endregion
                                break;
                            case E_PayPartner.Xrt:
                                var xrt_AppInfoConfig = LocalS.BLL.Biz.BizFactory.Merch.GetXrtPayInfoConfg(payTran.MerchId);
                                api_Result = SdkFactory.XrtPay.PayRefund(xrt_AppInfoConfig, order.PayTransId, rop.PayRefundId, payTran.ChargeAmount, rop.Amount, rop.Remark);
                                break;

                        }
                    }
                    else if (rop.Result == RopPayRefundHandle.E_Result.InVaild)
                    {
                        refundStatus = "INVAILD";
                    }

                    #endregion
                }
                else if (payRefund.ApplyMethod == E_PayRefundMethod.Manual)
                {
                    #region  Manual 线下人工处理
                    if (rop.Result == RopPayRefundHandle.E_Result.Success)
                    {
                        refundAmount = rop.Amount;
                        refundTime = DateTime.Now;
                        refundStatus = "SUCCESS";
                        refundRemark = "退款成功";
                    }
                    else if (rop.Result == RopPayRefundHandle.E_Result.Failure)
                    {
                        refundStatus = "FAIL";
                        refundRemark = "退款失败";
                    }
                    else if (rop.Result == RopPayRefundHandle.E_Result.InVaild)
                    {
                        refundStatus = "INVAILD";
                        refundRemark = "无效";
                    }

                    #endregion 
                }

                result = BizFactory.Order.PayRefundHandle(operater, rop.PayRefundId, refundStatus, refundAmount, refundTime, refundRemark, rop.Remark);

                CurrentDb.SaveChanges();
                ts.Complete();

                if (result.Result == ResultType.Success)
                {
                    if (refundStatus == "HANDLING")
                    {
                        Task4Factory.Tim2Global.Enter(Task4TimType.PayRefundCheckStatus, rop.PayRefundId, DateTime.Now.AddDays(3), new PayRefund2CheckStatusModel { Id = rop.PayRefundId, MerchId = order.MerchId, PayTransId = order.PayTransId, PayPartner = order.PayPartner });
                    }

                    MqFactory.Global.PushOperateLog(operater, AppId.MERCH, merchId, EventCode.pay_refund_handle, string.Format("订单号:{0}，处理退款金额：{1}，提交成功，退款单号：{2}", payRefund.OrderId, payRefund.ApplyAmount.ToF2Price(), payRefund.Id), rop);

                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "提交成功，结果稍后在退款查询查看");
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "提交失败");
                }

            }

            return result;

        }
    }
}