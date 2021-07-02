using LocalS.BLL.Biz;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using MyPushSdk;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocalS.BLL.Task
{
    public class Task4Mqtt2DeviceProvder : BaseService, ITask
    {
        private readonly string TAG = "Task4Mqtt2DeviceProvder";

        private EmqxPushService push;

        public CustomJsonResult Run()
        {
            CustomJsonResult result = new CustomJsonResult();

            push = new EmqxPushService();

            push.ConnectedEvent += ConnectedEvent;
            push.DisconnectedEvent += DisconnectedEvent;
            push.MessageReceivedEvent += MessageReceivedEvent;

            push.Connect();

            return result;
        }

        private void ConnectedEvent(object sender, EventArgs e)
        {
            LogUtil.Info(TAG, "服务器已连接");

            LogUtil.Info(TAG, "订阅主题：/topic_p_mch/#");

            //发布和回应主题
            push.SubscribeAsync(new List<TopicFilter> {
                    new TopicFilter("/topic_p_mch/#", MqttQualityOfServiceLevel.AtMostOnce),
                });
        }

        private void DisconnectedEvent(object sender, EventArgs e)
        {
            LogUtil.Info(TAG, "服务器已断开");

            System.Threading.Thread.Sleep(3000);

            LogUtil.Info(TAG, "尝试重新连接服务器");

            push.Connect();
        }

        private void MessageReceivedEvent(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

            LogUtil.Info(TAG, "接收到消息>>主题:" + topic + ",内容:" + payload);

            //服务器推送的消息到设备，到达确认
            if (topic.Contains("/topic_p_mch"))
            {
                Dictionary<string, object> obj_Payload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
                string id = obj_Payload["id"].ToString();
                string method = obj_Payload["method"].ToString();
                switch (method)
                {
                    case "msg_arrive":
                        msg_arrive(id);
                        break;
                }
            }

            ////设备推送的消息到服务，消息处理
            //if (topic.Contains("topic_p_mch"))
            //{
            //    Dictionary<string, JToken> msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, JToken>>(payload);

            //    if (msg.ContainsKey("type"))
            //    {
            //        string type = msg["type"].ToString();
            //        string deviceId = topic.Split('/')[1];
            //        string content = msg["content"].ToString(Newtonsoft.Json.Formatting.None);
            //        switch (type)
            //        {
            //            case "status":
            //                BizFactory.Device.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, deviceId, EventCode.DeviceStatus, "心跳包", content);
            //                break;
            //            case "pickup":
            //                BizFactory.Device.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, deviceId, EventCode.DevicePickup, "取货动作", content);
            //                break;
            //            case "pickup_test":
            //                BizFactory.Device.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, deviceId, EventCode.DevicePickupTest, "[测试]取货动作", content);
            //                break;
            //        }
            //    }
            //}
        }

        public void msg_arrive(string id)
        {
            var m_DeviceMqttMessage = CurrentDb.DeviceMqttMessage.Where(m => m.Id == id).FirstOrDefault();
            if (m_DeviceMqttMessage != null)
            {
                m_DeviceMqttMessage.IsArried = true;
                CurrentDb.SaveChanges();
                RedisManager.Db.StringSet("mqtt_msg:" + id, 1, new TimeSpan(0, 0, 60), StackExchange.Redis.When.Always);
            }
        }
    }
}
