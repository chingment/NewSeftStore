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

        public EmqxPushService()
        {

        }

        public void ConnectMqttServer()
        {
            if (mqttClient == null)
            {
                mqttClient = new MqttClientFactory().CreateMqttClient() as MqttClient;
                mqttClient.ApplicationMessageReceived += MqttClient_ApplicationMessageReceived;
                mqttClient.Connected += MqttClient_Connected;
                mqttClient.Disconnected += MqttClient_Disconnected;
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
                ConnectMqttServer();
            }

            if (!mqttClient.IsConnected)
            {
                ConnectMqttServer();
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

        private void MqttClient_Connected(object sender, EventArgs e)
        {
            LogUtil.Info(TAG, "服务器已连接");

            LogUtil.Info(TAG, "订阅主题：topic_p_mch/#，topic_r_mch/#");
            //发布和回应主题
            mqttClient.SubscribeAsync(new List<TopicFilter> {
                    new TopicFilter("topic_p_mch/#", MqttQualityOfServiceLevel.AtMostOnce),
                     new TopicFilter("topic_r_mch/#", MqttQualityOfServiceLevel.AtMostOnce),
                });
        }

        private void MqttClient_Disconnected(object sender, EventArgs e)
        {
            LogUtil.Info(TAG, "服务器已断开");

            System.Threading.Thread.Sleep(3000);

            LogUtil.Info(TAG, "尝试重新连接服务器");

            ConnectMqttServer();
        }

        private void MqttClient_ApplicationMessageReceived(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            LogUtil.Info(TAG, "接收到消息>>主题:" + topic + ",内容:" + payload);

            //服务器推送的消息到机器，到达确认
            if (topic.Contains("topic_r_mch"))
            {
                try
                {
                    Dictionary<string, string> msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(payload);

                    if (msg.ContainsKey("msg_id"))
                    {

                        RedisManager.Db.StringSet("msg:" + msg["msg_id"], 1, new TimeSpan(0, 0, 60), StackExchange.Redis.When.Always);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            //机器推送的消息到服务，消息处理
            if (topic.Contains("topic_p_mch"))
            {

            }

        }
    }
}
