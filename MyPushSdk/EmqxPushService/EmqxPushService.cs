using Jiguang.JPush;
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
    public class EmqxPushService : IPushService
    {
        private readonly string TAG = "EmqxPushService";

        private static MqttClient mqttClient = null;

        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceivedEvent;
        public event EventHandler ConnectedEvent;
        public event EventHandler DisconnectedEvent;

        public EmqxPushService()
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
                    CleanSession = true
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

        public CustomJsonResult Send(string registrationid, string type, object content)
        {
            if (mqttClient == null)
            {
                Connect();
            }

            if (!mqttClient.IsConnected)
            {
                Connect();
            }

            var result = new CustomJsonResult();

            string msg_id = Guid.NewGuid().ToString().Replace("-", "");
            string msg_type = type;

            var msg = new { msg_id = msg_id, type = msg_type, content = content };

            var appMsg = new MqttApplicationMessage("topic_s_mch/" + registrationid, Encoding.UTF8.GetBytes(JsonConvertUtil.SerializeObject(msg)), MqttQualityOfServiceLevel.AtMostOnce, false);

            var publish = mqttClient.PublishAsync(appMsg);

            //if (!publish.IsCompleted)
            //{
            //    return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "发送失败");
            //}


            var ret = new SendResult();

            ret.msg_id = msg_id;

            RedisManager.Db.StringSet("msg:" + msg_id, "0", new TimeSpan(0, 0, 60), StackExchange.Redis.When.Always);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "已发送，待确认", ret);

            return result;
        }

        public CustomJsonResult QueryStatus(string registrationid, string msgId)
        {
            var result = new CustomJsonResult();

            string msg = RedisManager.Db.StringGet("msg:" + msgId);

            if (msg == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "推送失败");

            if (msg != "1")
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "推送失败");

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "推送成功");

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
