using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.BLL.Mq.MqByRedis;
using LocalS.BLL.Push;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
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

        private MqttService mqtt;

        public CustomJsonResult Run()
        {
            CustomJsonResult result = new CustomJsonResult();

            mqtt = new MqttService();

            mqtt.ConnectedEvent += ConnectedEvent;
            mqtt.DisconnectedEvent += DisconnectedEvent;
            mqtt.MessageReceivedEvent += MessageReceivedEvent;
            mqtt.Connect();

            return result;
        }

        private void ConnectedEvent(object sender, EventArgs e)
        {
            LogUtil.Info(TAG, "服务器已连接");

            mqtt.SubscribeAsync(new List<TopicFilter> {
                    new TopicFilter("/+/+/user/update", MqttQualityOfServiceLevel.AtMostOnce),
                });
        }

        private void DisconnectedEvent(object sender, EventArgs e)
        {
            LogUtil.Info(TAG, "服务器已断开");

            System.Threading.Thread.Sleep(3000);

            LogUtil.Info(TAG, "尝试重新连接服务器");

            mqtt.Connect();
        }

        public static readonly object _lock = new object();

        private void MessageReceivedEvent(object sender, MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                string topic = e.ApplicationMessage.Topic;
                string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                LogUtil.Info(TAG, "接收到消息>>主题:" + topic + ",内容:" + payload);

                //服务器推送的消息到设备，到达确认
                if (topic.Contains("/user/update"))
                {
                    Dictionary<string, object> obj_Payload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);
                    string id = obj_Payload["id"].ToString();
                    string method = obj_Payload["method"].ToString();
                    string pms = null;

                    if (obj_Payload.ContainsKey("params"))
                    {
                        pms = obj_Payload["params"].ToJsonString();
                    }

                    string deviceId = topic.Split('/')[2];

                    LogUtil.Info(TAG, "接收到消息>>deviceId:" + deviceId);

                    switch (method)
                    {
                        case "msg_arrive":
                            msg_arrive(id);
                            break;
                        case "device_status":
                            BizFactory.Device.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, deviceId, EventCode.device_status, "心跳包", pms);
                            break;
                        case "vending_pickup":
                            BizFactory.Device.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, deviceId, EventCode.vending_pickup, "取货动作", pms);
                            break;
                        case "vending_pickup_test":
                            BizFactory.Device.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, deviceId, EventCode.vending_pickup_test, "[测试]取货动作", pms);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(TAG, ex);
            }
        }

        public void msg_arrive(string id)
        {
            LogUtil.Info(TAG, "msg_arrive:" + id);

            var m_DeviceMqttMessage = CurrentDb.DeviceMqttMessage.Where(m => m.Id == id).FirstOrDefault();
            if (m_DeviceMqttMessage != null)
            {
                m_DeviceMqttMessage.IsArried = true;
                CurrentDb.SaveChanges();
                RedisManager.Db.StringSet("mqtt_msg:" + id, "1", new TimeSpan(0, 0, 60), StackExchange.Redis.When.Always);

                LogUtil.Info(TAG, "msg_arrive:" + id + ",SaveChanges");
            }
        }
    }
}
