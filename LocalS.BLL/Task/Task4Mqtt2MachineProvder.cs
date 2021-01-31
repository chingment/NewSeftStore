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
    public class Task4Mqtt2MachineProvder : BaseService, ITask
    {
        private readonly string TAG = "Task4Mqtt2MachineProvder";

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

            LogUtil.Info(TAG, "订阅主题：topic_p_mch/#，topic_r_mch/#");

            //发布和回应主题
            push.SubscribeAsync(new List<TopicFilter> {
                    new TopicFilter("topic_p_mch/#", MqttQualityOfServiceLevel.AtMostOnce),
                     new TopicFilter("topic_r_mch/#", MqttQualityOfServiceLevel.AtMostOnce),
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
                Dictionary<string, JToken> msg = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, JToken>>(payload);

                if (msg.ContainsKey("type"))
                {
                    string type = msg["type"].ToString();
                    string machineId = topic.Split('/')[1];
                    string content = msg["content"].ToString(Newtonsoft.Json.Formatting.None);
                    switch (type)
                    {
                        case "machine_status":
                            BizFactory.Machine.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, machineId, EventCode.MachineStatus, "心跳包", content);
                            break;
                        case "pickup_action":
                            BizFactory.Machine.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, machineId, EventCode.Pickup, "取货动作", content);
                            break;
                        case "pickup_test":
                            BizFactory.Machine.EventNotify(IdWorker.Build(IdType.EmptyGuid), AppId.STORETERM, machineId, EventCode.PickupTest, "[测试]取货动作", content);
                            break;
                    }
                }
            }
        }
    }
}
