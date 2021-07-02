using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using MQTTnet.Core.Client;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Push
{
    public class PushService : BaseService
    {
        private static string TAG = "PushService";
        private EmqxPushService push;
        private static PushService pushService = null;

        private EmqxPushService Push
        {
            get
            {
                return push;
            }
        }

        private PushService()
        {
            push = new EmqxPushService();
            push.ConnectedEvent += ConnectedEvent;
            push.DisconnectedEvent += DisconnectedEvent;
            push.MessageReceivedEvent += MessageReceivedEvent;
        }

        public static PushService GetInstance()
        {
            if (pushService == null)
            {
                pushService = new PushService();
            }

            return pushService;
        }

        private void ConnectedEvent(object sender, EventArgs e)
        {
            LogUtil.Info(TAG, "服务器已连接");
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
        }

        private CustomJsonResult Send(string operater, string appId, string merchId, string deviceId, string method, object prms)
        {
            var d_DeviceMqttMessage = new DeviceMqttMessage();
            d_DeviceMqttMessage.Id = IdWorker.Build(IdType.NewGuid);
            d_DeviceMqttMessage.MerchId = merchId;
            d_DeviceMqttMessage.DeviceId = deviceId;
            d_DeviceMqttMessage.Method = method;
            d_DeviceMqttMessage.Params = prms.ToJsonString();
            d_DeviceMqttMessage.IsArried = false;
            d_DeviceMqttMessage.Version = "1.0.0.0";
            d_DeviceMqttMessage.Creator = operater;
            d_DeviceMqttMessage.CreateTime = DateTime.Now;
            CurrentDb.DeviceMqttMessage.Add(d_DeviceMqttMessage);
            CurrentDb.SaveChanges();

            var result = GetInstance().Push.Send(deviceId, d_DeviceMqttMessage.Id, method, prms);

            return result;
        }

        public static CustomJsonResult SendUpdateStock(string operater, string appId, string merchId, string deviceId, object prms)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, "update_stock", prms);
            return result;
        }

        public static CustomJsonResult SendUpdateAds(string operater, string appId, string merchId, string deviceId, object prms)
        {
            var result = GetInstance().Send(operater, appId, merchId, deviceId, "update_ads", prms);
            return result;
        }

        public static CustomJsonResult SendUpdateHomeLogo(string operater, string appId, string merchId, string deviceId, object prms)
        {
            var result = GetInstance().Send(operater, appId, merchId, deviceId, "update_home_logo", prms);
            return result;
        }

        public static CustomJsonResult SendRebootSys(string operater, string appId, string merchId, string deviceId)
        {
            var result = GetInstance().Send(operater, appId, merchId, deviceId, "reboot_sys", null);
            return result;
        }

        public static CustomJsonResult SendShutdownSys(string operater, string appId, string merchId, string deviceId)
        {
            var result = GetInstance().Send(operater, appId, merchId, deviceId, "shutdown_sys", null);
            return result;
        }

        public static CustomJsonResult SendSetSysStatus(string operater, string appId, string merchId, string deviceId, object prms)
        {
            var result = GetInstance().Send(operater, appId, merchId, deviceId, "set_sys_status", prms);
            return result;
        }

        public static CustomJsonResult SendOpenPickupDoor(string operater, string appId, string merchId, string deviceId)
        {
            var result = GetInstance().Send(operater, appId, merchId, deviceId, "open_pickup_door", null);
            return result;
        }

        public static CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string deviceId, object prms)
        {
            var result = GetInstance().Send(operater, appId, merchId, deviceId, "pay_success", prms);
            return result;
        }

        public static CustomJsonResult QueryStatus(string operater, string appId, string merchId, string deviceId, string messageId)
        {
            var result = GetInstance().Push.QueryStatus(deviceId, messageId);
            return result;
        }


    }
}
