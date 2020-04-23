﻿using Jiguang.JPush;
using Jiguang.JPush.Model;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPushSdk
{
    public class JgPushService : IPushService
    {
        private static JPushClient client = new JPushClient("47571aa2482f3b9e2af243a9", "8b0ea490c90fddbf64e0fb9f");

        public CustomJsonResult Send( string registrationid, string cmd, object content)
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

            var response = client.SendPush(pushPayload);


            //todo 查询 var response = client.Report.GetMessageSendStatus() 推送状态
            return result;
        }
    }
}
