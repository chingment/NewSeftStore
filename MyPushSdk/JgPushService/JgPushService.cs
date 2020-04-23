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
    public class JgPushResult
    {
        public string sendno { get; set; }

        public string msg_id { get; set; }
    }

    public class JgPushQuqeryResult
    {
        public string android_received { get; set; }
        public string ios_apns_received { get; set; }
        public string ios_apns_sent { get; set; }
        public string ios_msg_received { get; set; }
        public string msg_id { get; set; }
        public string wp_mpns_sent { get; set; }
    }

    public class JgPushService : IPushService
    {
        private static JPushClient client = new JPushClient("47571aa2482f3b9e2af243a9", "8b0ea490c90fddbf64e0fb9f");

        public CustomJsonResult Send(string registrationid, string cmd, object content)
        {
            var result = new CustomJsonResult();


            if (string.IsNullOrEmpty(registrationid))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "registrationid 不能为空");
            }

            if (string.IsNullOrEmpty(cmd))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "cmd 不能为空");
            }


            if (content == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "content 不能为空");
            }

            try
            {

                Dictionary<String, Object> audience = new Dictionary<string, object>();
                List<string> registration_id = new List<string>();
                registration_id.Add(registrationid);
                audience.Add("registration_id", registration_id);

                PushPayload pushPayload = new PushPayload()
                {
                    Platform = new List<string> { "android" },
                    Audience = audience,//Audience ="all" 全部
                    Message = new Message
                    {
                        Title = "",
                        Content = "",
                        Extras = new Dictionary<string, string>
                        {
                            ["cmd"] = cmd,
                            ["content"] = content.ToJsonString()
                        }
                    },
                    Options = new Options
                    {
                        IsApnsProduction = true // 设置 iOS 推送生产环境。不设置默认为开发环境。
                    }
                };

                LogUtil.Info("registration_id:" + registration_id);
                LogUtil.Info("pushPayload:" + pushPayload.ToJsonString());

                var response = client.SendPush(pushPayload);

                //"{\"sendno\":\"0\",\"msg_id\":\"9007254936655942\"}"

                //List<string> a = new List<string>();
                //a.Add("9007254792504437");
                //var sss = client.Report.GetMessageReport(a);

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    //if (response.Content != null)
                    //{
                    //    System.Threading.Thread.Sleep(2000);

                    //    var jgPushResult = response.Content.ToJsonObject<JgPushResult>();
                    //    if (jgPushResult != null)
                    //    {
                    //        List<string> msgIds = new List<string>();
                    //        msgIds.Add(jgPushResult.msg_id);

                    //        var jgReportResult = client.Report.GetMessageReport(msgIds);
                    //        if (jgReportResult != null)
                    //        {
                    //            if (!string.IsNullOrEmpty(jgReportResult.Content))
                    //            {
                    //                //[{"android_received":null,"ios_apns_received":null,"ios_apns_sent":null,"ios_msg_received":null,"msg_id":"47287851766226406","wp_mpns_sent":null}]

                    //                var jgPushQuqeryResult = jgReportResult.Content.ToJsonObject<List<JgPushQuqeryResult>>();

                    //                if (jgPushQuqeryResult != null)
                    //                {
                    //                    var obj = jgPushQuqeryResult.Where(m => m.msg_id == jgPushResult.msg_id).FirstOrDefault();

                    //                    if (obj.android_received != null)
                    //                    {
                    //                        result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "推送成功");
                    //                        return result;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发送成功");
                }
                else
                {
                    result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "发送失败");
                }
            }
            catch (Exception ex)
            {
                result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发送异常：" + ex.Message);
            }

            //todo 查询 var response = client.Report.GetMessageSendStatus() 推送状态
            return result;
        }
    }
}
