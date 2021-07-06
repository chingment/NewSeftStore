﻿using Jiguang.JPush;
using Jiguang.JPush.Model;
using Lumos;
using Lumos.Redis;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPushSdk
{
    public class MqttService : IPushService
    {
        private readonly string TAG = "MqttService";

        private static MqttClient mqttClient = null;

        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceivedEvent;
        public event EventHandler ConnectedEvent;
        public event EventHandler DisconnectedEvent;

        public MqttService()
        {

        }

        public void Connect()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
                mqttClient.ApplicationMessageReceived += MessageReceivedEvent;
                mqttClient.Connected += ConnectedEvent;
                mqttClient.Disconnected += DisconnectedEvent;
            }

            try
            {
                var options = new MqttClientTcpOptions
                {
                    Server = "112.74.179.185",
                    Port = 1883,
                    ClientId = Guid.NewGuid().ToString().Substring(0, 5),
                    UserName = "admin",
                    Password = "public",
                    CleanSession = true,
                };

                if (!mqttClient.IsConnected)
                {
                    var connect = mqttClient.ConnectAsync(options);


                }
            }
            catch (Exception ex)
            {
                LogUtil.Error($"连接到MQTT服务器失败！" + Environment.NewLine + ex.Message + Environment.NewLine);
            }
        }

        public CustomJsonResult Send(string deviceId, string msgId, string method, object pms)
        {
            LogUtil.Info(TAG, "开始发送命令");

            if (mqttClient == null)
            {
                Connect();
            }

            if (!mqttClient.IsConnected)
            {
                Connect();
            }

            if (!mqttClient.IsConnected)
            {
                LogUtil.Info(TAG, "连接失败");

                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "消息服务器连接失败");
            }

            var result = new CustomJsonResult();

            var obj_payload = new { Id = msgId, Method = method, Params = pms };
            var str_payload = JsonConvertUtil.SerializeObject(obj_payload);

            var topic = "/a1A2Mq6w5ln/" + deviceId + "/user/get";

            LogUtil.Info(TAG, "topic:" + topic);

            var appMsg = new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(str_payload), MqttQualityOfServiceLevel.AtMostOnce, false);
            var publish = mqttClient.PublishAsync(appMsg);
          


            //if (!publish.IsCompleted)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发送失败");
            //}


            RedisManager.Db.StringSet("mqtt_msg:" + msgId, "0", new TimeSpan(0, 0, 60), StackExchange.Redis.When.Always);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "已发送，待确认", new { msgId = msgId });

            return result;
        }

        public CustomJsonResult QueryStatus(string registrationid, string msgId)
        {
            var result = new CustomJsonResult();

            string msg = RedisManager.Db.StringGet("mqtt_msg:" + msgId);

            if (msg == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未响应，命令发送失败");
            }

            if (msg != "1")
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未响应，命令发送失败");

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "命令发送成功");

            return result;
        }

        public Task<IList<MqttSubscribeResult>> SubscribeAsync(IEnumerable<TopicFilter> topicFilters)
        {
            if (mqttClient == null)
                return null;

            return mqttClient.SubscribeAsync(topicFilters);
        }
    }
}