using Jiguang.JPush;
using Jiguang.JPush.Model;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPushSdk
{
    //public class JgPushService : IPushService
    //{
    //    private static JPushClient client = new JPushClient("47571aa2482f3b9e2af243a9", "8b0ea490c90fddbf64e0fb9f");

    //    public CustomJsonResult Send(string registrationid, string cmd, object content)
    //    {
    //        var result = new CustomJsonResult();


    //        if (string.IsNullOrEmpty(registrationid))
    //        {
    //            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "registrationid 不能为空");
    //        }

    //        if (string.IsNullOrEmpty(cmd))
    //        {
    //            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "cmd 不能为空");
    //        }


    //        if (content == null)
    //        {
    //            return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "content 不能为空");
    //        }

    //        try
    //        {

    //            Dictionary<String, Object> audience = new Dictionary<string, object>();
    //            List<string> registration_id = new List<string>();
    //            registration_id.Add(registrationid);
    //            audience.Add("registration_id", registration_id);

    //            PushPayload pushPayload = new PushPayload()
    //            {
    //                Platform = new List<string> { "android" },
    //                Audience = audience,//Audience ="all" 全部
    //                Message = new Message
    //                {
    //                    Title = "",
    //                    Content = "",
    //                    Extras = new Dictionary<string, string>
    //                    {
    //                        ["cmd"] = cmd,
    //                        ["content"] = content.ToJsonString()
    //                    }
    //                },
    //                Options = new Options
    //                {
    //                    IsApnsProduction = true // 设置 iOS 推送生产环境。不设置默认为开发环境。
    //                }
    //            };

    //            LogUtil.Info("registration_id:" + registration_id);
    //            LogUtil.Info("pushPayload:" + pushPayload.ToJsonString());

    //            var response = client.SendPush(pushPayload);

    //            if (response.StatusCode == System.Net.HttpStatusCode.OK)
    //            {
    //                var jgPushResult = response.Content.ToJsonObject<SendResult>();

    //                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发送成功", new { messageId = jgPushResult.msg_id });
    //            }
    //            else
    //            {
    //                result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发送失败");
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发送异常：" + ex.Message);
    //        }

    //        //todo 查询 var response = client.Report.GetMessageSendStatus() 推送状态
    //        return result;
    //    }

    //    public CustomJsonResult QueryStatus(string registrationid, string msgId)
    //    {
    //        var result = new CustomJsonResult();

    //        List<string> registrationids = new List<string>();
    //        registrationids.Add(registrationid);

    //        var jgReportResult = client.Report.GetMessageSendStatus(msgId, registrationids, null);
    //        if (jgReportResult != null)
    //        {
    //            if (!string.IsNullOrEmpty(jgReportResult.Content))
    //            {
    //                LogUtil.Info("推送结果：" + jgReportResult.Content);
    //                //{ "1104a8979234f30c8c2":{ "status":0} }
    //                //[{"android_received":null,"ios_apns_received":null,"ios_apns_sent":null,"ios_msg_received":null,"msg_id":"47287851766226406","wp_mpns_sent":null}]

    //                var jgPushQuqeryResult = jgReportResult.Content.ToJsonObject<Dictionary<string, QueryStatusResult>>();

    //                if (jgPushQuqeryResult.ContainsKey(registrationid))
    //                {
    //                    int status = jgPushQuqeryResult[registrationid].status;

    //                    if (status == 0)
    //                    {
    //                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "推送成功");
    //                        return result;
    //                    }
    //                }
    //                //if (jgPushQuqeryResult != null)
    //                //{
    //                //    var obj = jgPushQuqeryResult.Where(m => m.msg_id == msgId).FirstOrDefault();
    //                //    if (obj.android_received != null)
    //                //    {
    //                //        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "推送成功", obj);
    //                //        return result;
    //                //    }
    //                //}
    //            }
    //        }


    //        result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "推送失败");
    //        return result;
    //    }
    //}
}
