using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using MQTTnet.Core.Client;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Push
{
    public class PushService
    {
        private static string TAG = "PushService";
        private EmqxPushService push;
        private static PushService pushService = null;
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

        public CustomJsonResult Send(string operater, string appId, string merchId, string deviceId, string cmd, object content)
        {
            LogUtil.Info("Send1");
            var result = new CustomJsonResult();
            result = GetInstance().push.Send(deviceId, cmd, content);

            return result;
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

        public static CustomJsonResult SendStock(string operater, string appId, string merchId, string deviceId, object content)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdUpdateSkuStock, content);
            return result;
        }

        public static CustomJsonResult SendAds(string operater, string appId, string merchId, string deviceId, object content)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdUpdateAds, content);
            return result;
        }

        public static CustomJsonResult SendHomeLogo(string operater, string appId, string merchId, string deviceId, object content)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdUpdateHomeLogo, content);
            return result;
        }

        public static CustomJsonResult SendSysReboot(string operater, string appId, string merchId, string deviceId)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdSysReboot, "重启系统");
            return result;
        }

        public static CustomJsonResult SendSysShutdown(string operater, string appId, string merchId, string deviceId)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdSysShutdown, "关闭系统");
            return result;
        }

        public static CustomJsonResult SendSysSetStatus(string operater, string appId, string merchId, string deviceId, object content)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdSysSetStatus, content);
            return result;
        }

        public static CustomJsonResult SendDsx01OpenPickupDoor(string operater, string appId, string merchId, string deviceId)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdDsx01OpenPickupDoor, "打开取货门");
            return result;
        }

        public static CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string deviceId, object content)
        {
            var result = new CustomJsonResult();
            result = GetInstance().Send(operater, appId, merchId, deviceId, EventCode.MCmdPaySuccess, content);
            return result;
        }

        public static CustomJsonResult QueryStatus(string operater, string appId, string merchId, string deviceId, string messageId)
        {
            var result = new CustomJsonResult();
            var lDevice = BizFactory.Device.GetOne(deviceId);
            result = GetInstance().push.QueryStatus("", messageId);
            return result;
        }


    }
}
