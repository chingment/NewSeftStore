using LocalS.BLL;
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



            var order = BizFactory.Order.GetOne(rup.Id);

            if (order == null)
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
                //switch (order.Status)
                //{
                //    case E_OrderStatus.Submitted:
                //        ret.Result = RetOperateResult.ResultType.Success;
                //        ret.Message = "该订单未支付";
                //        ret.IsComplete = true;
                //        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
                //        break;
                //    case E_OrderStatus.WaitPay:
                //        ret.Result = RetOperateResult.ResultType.Success;
                //        ret.IsComplete = false;
                //        ret.Message = "该订单未支付";
                //        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
                //        break;
                //    case E_OrderStatus.Payed:
                //        ret.Result = RetOperateResult.ResultType.Success;
                //        ret.Remarks = "";
                //        ret.Message = "支付成功";
                //        ret.IsComplete = true;

                //        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "回到首页", Color = "red" }, OpType = "FUN", OpVal = "goHome" });
                //        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = GetOrderDetailsUrl(rup.Caller, order.Id, order.Status) });

                //        ret.Fields.Add(new FsField("订单号", "", order.Id, ""));
                //        ret.Fields.Add(new FsField("提交时间", "", order.SubmittedTime.ToUnifiedFormatDateTime(), ""));
                //        ret.Fields.Add(new FsField("支付时间", "", order.PayedTime.ToUnifiedFormatDateTime(), ""));

                //        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "支付成功", ret);
                //        break;
                //    case E_OrderStatus.Completed:
                //        ret.Result = RetOperateResult.ResultType.Success;
                //        ret.Message = "该订单已经完成";
                //        ret.IsComplete = true;

                //        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "回到首页", Color = "red" }, OpType = "FUN", OpVal = "goHome" });
                //        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = GetOrderDetailsUrl(rup.Caller, order.Id, order.Status) });

                //        ret.Fields.Add(new FsField("订单号", "", order.Id, ""));
                //        ret.Fields.Add(new FsField("提交时间", "", order.SubmittedTime.ToUnifiedFormatDateTime(), ""));
                //        ret.Fields.Add(new FsField("支付时间", "", order.PayedTime.ToUnifiedFormatDateTime(), ""));
                //        ret.Fields.Add(new FsField("完成时间", "", order.CompletedTime.ToUnifiedFormatDateTime(), ""));

                //        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单已经完成", ret);
                //        break;
                //    case E_OrderStatus.Canceled:
                //        ret.Result = RetOperateResult.ResultType.Success;
                //        ret.Message = "该订单已经取消";
                //        ret.IsComplete = true;

                //        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "回到首页", Color = "red" }, OpType = "FUN", OpVal = "goHome" });
                //        ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "查看详情", Color = "green" }, OpType = "URL", OpVal = GetOrderDetailsUrl(rup.Caller, order.Id, order.Status) });

                //        ret.Fields.Add(new FsField("订单号", "", order.Id, ""));
                //        ret.Fields.Add(new FsField("提交时间", "", order.SubmittedTime.ToUnifiedFormatDateTime(), ""));
                //        ret.Fields.Add(new FsField("取消时间", "", order.CanceledTime.ToUnifiedFormatDateTime(), ""));
                //        ret.Fields.Add(new FsField("取消原因", "", order.CancelReason, ""));

                //        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单已经取消", ret);
                //        break;
                //    default:
                //        break;
                //}
            }



            return result;
        }

        private CustomJsonResult SendPayCancleResult(string operater, string clientUserId, RupOperateResult rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetOperateResult();

            var order = BizFactory.Order.GetOne(rup.Id);

            if (order == null)
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
                ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "回到首页", Color = "red" }, OpType = "FUN", OpVal = "goHome" });
                //ret.Buttons.Add(new FsButton() { Name = new FsText() { Content = "继续支付", Color = "green" }, OpType = "URL", OpVal = GetOrderDetailsUrl(rup.Caller, order.Id, order.Status) });
                ret.Fields.Add(new FsField("订单号", "", order.Id, ""));
                ret.Fields.Add(new FsField("提交时间", "", order.SubmittedTime.ToUnifiedFormatDateTime(), ""));

                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "订单未支付", ret);
            }

            return result;
        }

        public static string GetOrderDetailsUrl(E_AppCaller caller, string orderid, E_OrderStatus orderStatus)
        {
            string url = "";
            switch (caller)
            {
                case E_AppCaller.Wxmp:
                    switch (orderStatus)
                    {
                        case E_OrderStatus.WaitPay:
                            url = string.Format("/pages/orderconfirm/orderconfirm?orderId={0}", orderid);
                            break;
                        default:
                            url = string.Format("/pages/orderdetails/orderdetails?id={0}", orderid);
                            break;
                    }
                    break;
            }

            return url;
        }
    }
}
