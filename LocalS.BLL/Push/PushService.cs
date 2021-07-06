using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
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

namespace LocalS.BLL.Push
{

    public class MqttService : BaseService
    {
        private readonly string TAG = "MqttService";
        private static MqttService mqttService = null;
        private static MqttClient mqttClient = null;

        public event EventHandler<MqttApplicationMessageReceivedEventArgs> MessageReceivedEvent;
        public event EventHandler ConnectedEvent;
        public event EventHandler DisconnectedEvent;

        public MqttService()
        {

        }

        public static MqttService GetInstance()
        {
            if (mqttService == null)
            {
                mqttService = new MqttService();
            }

            return mqttService;
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

        public CustomJsonResult Send(string operater, string appId, string merchId, string deviceId, string method, object pms)
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

            string msgId = IdWorker.Build(IdType.NewGuid);
            var d_DeviceMqttMessage = new DeviceMqttMessage();
            d_DeviceMqttMessage.Id = msgId;
            d_DeviceMqttMessage.MerchId = merchId;
            d_DeviceMqttMessage.DeviceId = deviceId;
            d_DeviceMqttMessage.Method = method;
            d_DeviceMqttMessage.Params = pms.ToJsonString();
            d_DeviceMqttMessage.IsArried = false;
            d_DeviceMqttMessage.Version = "1.0.0.0";
            d_DeviceMqttMessage.Creator = operater;
            d_DeviceMqttMessage.CreateTime = DateTime.Now;
            CurrentDb.DeviceMqttMessage.Add(d_DeviceMqttMessage);
            CurrentDb.SaveChanges();

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

        public CustomJsonResult QueryStatus(string operater, string appId, string merchId, string deviceId, string msgId)
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

    //public class PushService : BaseService
    //{
    //    private static string TAG = "PushService";
    //    private MqttService mqtt;
    //    private static PushService pushService = null;

    //    private PushService()
    //    {
    //        mqtt = new MqttService();
    //        mqtt.ConnectedEvent += ConnectedEvent;
    //        mqtt.DisconnectedEvent += DisconnectedEvent;
    //        mqtt.MessageReceivedEvent += MessageReceivedEvent;
    //    }

    //    public static PushService GetInstance()
    //    {
    //        if (pushService == null)
    //        {
    //            pushService = new PushService();
    //        }

    //        return pushService;
    //    }

    //    private void ConnectedEvent(object sender, EventArgs e)
    //    {
    //        LogUtil.Info(TAG, "服务器已连接");
    //    }

    //    private void DisconnectedEvent(object sender, EventArgs e)
    //    {
    //        LogUtil.Info(TAG, "服务器已断开");

    //        System.Threading.Thread.Sleep(3000);

    //        LogUtil.Info(TAG, "尝试重新连接服务器");

    //        mqtt.Connect();
    //    }

    //    private void MessageReceivedEvent(object sender, MqttApplicationMessageReceivedEventArgs e)
    //    {
    //        string topic = e.ApplicationMessage.Topic;
    //        string payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

    //        LogUtil.Info(TAG, "接收到消息>>主题:" + topic + ",内容:" + payload);
    //    }

    //    public CustomJsonResult Send(string operater, string appId, string merchId, string deviceId, string method, object prms)
    //    {
    //        var d_DeviceMqttMessage = new DeviceMqttMessage();
    //        d_DeviceMqttMessage.Id = IdWorker.Build(IdType.NewGuid);
    //        d_DeviceMqttMessage.MerchId = merchId;
    //        d_DeviceMqttMessage.DeviceId = deviceId;
    //        d_DeviceMqttMessage.Method = method;
    //        d_DeviceMqttMessage.Params = prms.ToJsonString();
    //        d_DeviceMqttMessage.IsArried = false;
    //        d_DeviceMqttMessage.Version = "1.0.0.0";
    //        d_DeviceMqttMessage.Creator = operater;
    //        d_DeviceMqttMessage.CreateTime = DateTime.Now;
    //        CurrentDb.DeviceMqttMessage.Add(d_DeviceMqttMessage);
    //        CurrentDb.SaveChanges();

    //        var result = GetInstance().mqtt.Send(deviceId, d_DeviceMqttMessage.Id, method, prms);

    //        return result;
    //    }

    //    public static CustomJsonResult QueryStatus(string operater, string appId, string merchId, string deviceId, string messageId)
    //    {
    //        var result = GetInstance().mqtt.QueryStatus(deviceId, messageId);
    //        return result;
    //    }


    //}
}
