﻿using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using LocalS.Service.UI;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class OperateService : BaseDbContext
    {
        public CustomJsonResult Result(string operater, string clientUserId, RupOperateResult rup)
        {
            var result = new CustomJsonResult();

            switch (rup.Type)
            {
                case E_OperateType.SendPaySuccessCheck:
                    result = SendPaySuccessResult(operater, clientUserId, rup);
                    break;
                case E_OperateType.SendPayCancleCheck:
                    result = SendPayCancleResult(operater, clientUserId, rup);
                    break;
            }

            return result;
        }


        private CustomJsonResult SendPaySuccessResult(string operater, string clientUserId, RupOperateResult rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOperateResult();

            var payTrans = CurrentDb.PayTrans.Where(m => m.Id == rup.Id).FirstOrDefault();

            if (payTrans == null)
            {
                ret.Result = RetOperateResult.ResultType.Failure;
                ret.Message = "系统找不到该订单号";
                ret.IsComplete = true;
                ret.IsShowContactButton = true;
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "查询支付结果失败：找不到该订单", ret);
            }

            if (rup.IsTimeOut)
            {
                ret.Result = RetOperateResult.ResultType.Exception;
                ret.Message = "处理超时......";
                ret.IsComplete = true;
                ret.IsShowContactButton = true;
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理超时......", ret);
            }
            else
            {
                switch (payTrans.PayStatus)
                {
                    case E_PayStatus.WaitPay:
                        ret.Result = RetOperateResult.ResultType.Success;
                        ret.IsComplete = false;
                        ret.Message = "该订单未支付";
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
                        break;
                    case E_PayStatus.Paying:
                        ret.Result = RetOperateResult.ResultType.Success;
                        ret.IsComplete = false;
                        ret.Message = "该订单正在支付";
                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "该订单正在支付", ret);
                        break;
                    case E_PayStatus.PaySuccess:
                        ret.Result = RetOperateResult.ResultType.Success;
                        ret.Remarks = "";
                        ret.Message = "支付成功";
                        ret.IsComplete = true;

                        string action = rup.Action.ToLower();

                        switch (action)
                        {
                            case "memberfee":
                                ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "会员中心", Color = "red" }, OpType = "URL", OpVal = GetMemberPromUrl(rup.Caller) });
                                break;
                            default:
                                ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "回到首页", Color = "red" }, OpType = "FUN", OpVal = "goHome" });
                                break;
                        }


                        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = GetOrderDetailsUrl(rup.Caller, payTrans.OrderIds, payTrans.PayStatus) });

                        ret.Fields.Add(new FsField("交易号", "", payTrans.Id, ""));
                        ret.Fields.Add(new FsField("提交时间", "", payTrans.SubmittedTime.ToUnifiedFormatDateTime(), ""));
                        ret.Fields.Add(new FsField("支付时间", "", payTrans.PayedTime.ToUnifiedFormatDateTime(), ""));

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "支付成功", ret);
                        break;
                    case E_PayStatus.PayCancle:
                    case E_PayStatus.PayTimeout:
                        ret.Result = RetOperateResult.ResultType.Success;
                        ret.Message = "该订单已经取消";
                        ret.IsComplete = true;

                        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "回到首页", Color = "red" }, OpType = "FUN", OpVal = "goHome" });
                        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = GetOrderDetailsUrl(rup.Caller, payTrans.OrderIds, payTrans.PayStatus) });

                        ret.Fields.Add(new FsField("交易号", "", payTrans.Id, ""));
                        ret.Fields.Add(new FsField("提交时间", "", payTrans.SubmittedTime.ToUnifiedFormatDateTime(), ""));
                        ret.Fields.Add(new FsField("取消时间", "", payTrans.CanceledTime.ToUnifiedFormatDateTime(), ""));
                        ret.Fields.Add(new FsField("取消原因", "", payTrans.CancelReason, ""));

                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单已经取消", ret);
                        break;
                    default:
                        break;
                }
            }



            return result;
        }

        private CustomJsonResult SendPayCancleResult(string operater, string clientUserId, RupOperateResult rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOperateResult();

            var payTrans = CurrentDb.PayTrans.Where(m => m.Id == rup.Id).FirstOrDefault();

            if (payTrans == null)
            {
                ret.Result = RetOperateResult.ResultType.Failure;
                ret.Message = "系统找不到该订单号";
                ret.IsComplete = true;
                ret.IsShowContactButton = true;
                return new CustomJsonResult(ResultType.Success, ResultCode.Success, "查询支付结果失败：找不到该订单", ret);
            }

            if (rup.IsTimeOut)
            {
                ret.Result = RetOperateResult.ResultType.Exception;
                ret.Message = "处理超时......";
                ret.IsComplete = true;
                ret.IsShowContactButton = true;
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理超时......", ret);
            }
            else
            {
                ret.Result = RetOperateResult.ResultType.Tips;
                ret.IsComplete = true;
                ret.Message = "您已取消支付操作";

                string action = rup.Action.ToLower();

                switch (action)
                {
                    case "memberfee":
                        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "会员中心", Color = "red" }, OpType = "URL", OpVal = GetMemberPromUrl(rup.Caller) });
                        break;
                    default:
                        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "回到首页", Color = "red" }, OpType = "FUN", OpVal = "goHome" });
                        break;
                }


                ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "继续支付", Color = "green" }, OpType = "URL", OpVal = GetOrderDetailsUrl(rup.Caller, payTrans.OrderIds, payTrans.PayStatus) });
                ret.Fields.Add(new FsField("交易号", "", payTrans.Id, ""));
                ret.Fields.Add(new FsField("提交时间", "", payTrans.SubmittedTime.ToUnifiedFormatDateTime(), ""));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
            }

            return result;
        }

        public static string GetOrderDetailsUrl(E_AppCaller caller, string orderIds, E_PayStatus payStatus)
        {
            string url = "";
            switch (caller)
            {
                case E_AppCaller.Wxmp:
                    switch (payStatus)
                    {
                        case E_PayStatus.WaitPay:
                        case E_PayStatus.Paying:
                            url = string.Format("/pages/orderconfirm/orderconfirm?orderIds={0}", orderIds);
                            break;
                        default:
                            url = string.Format("/pages/orderdetails/orderdetails?id={0}", orderIds);
                            break;
                    }
                    break;
            }

            return url;
        }

        public static string GetMemberCenterUrl(E_AppCaller caller)
        {
            string url = "";
            switch (caller)
            {
                case E_AppCaller.Wxmp:
                    url = "/pages/membercenter/membercenter";
                    break;
            }

            return url;
        }

        public static string GetMemberPromUrl(E_AppCaller caller)
        {
            string url = "";
            switch (caller)
            {
                case E_AppCaller.Wxmp:
                    url = "/pages/memberprom/memberprom";
                    break;
            }

            return url;
        }


    }
}
