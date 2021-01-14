using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Push
{
    public static class PushService
    {
        private static IPushService pushService = new EmqxPushService();

        public static CustomJsonResult Send(string operater, string appId, string merchId, string machineId, string cmd, object content)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(machineId);

            if (machine != null)
            {
                result = pushService.Send(machineId, cmd, content);

                if (result.Result == ResultType.Success)
                {
                    MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, machine.StoreId, machine.ShopId, machineId, cmd, "命令发送成功");
                }
                else
                {
                    MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, machine.StoreId, machine.ShopId, machineId, cmd, "命令发送失败");
                }
            }

            return result;
        }

        public static CustomJsonResult SendStock(string operater, string appId, string merchId, string machineId, object content)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdUpdateProductSkuStock, content);
            return result;
        }

        public static CustomJsonResult SendUpdateMachineHomeBanners(string operater, string appId, string merchId, string machineId, object content)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdUpdateHomeBanners, content);
            return result;
        }

        public static CustomJsonResult SendUpdateMachineHomeLogo(string operater, string appId, string merchId, string machineId, object content)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdUpdateHomeLogo, content);
            return result;
        }

        public static CustomJsonResult SendSysReboot(string operater, string appId, string merchId, string machineId)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdSysReboot, "重启系统");
            return result;
        }

        public static CustomJsonResult SendSysShutdown(string operater, string appId, string merchId, string machineId)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdSysShutdown, "关闭系统");
            return result;
        }

        public static CustomJsonResult SendSysSetStatus(string operater, string appId, string merchId, string machineId, object content)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdSysSetStatus, content);
            return result;
        }

        public static CustomJsonResult SendDsx01OpenPickupDoor(string operater, string appId, string merchId, string machineId)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdDsx01OpenPickupDoor, "打开取货门");
            return result;
        }

        public static CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string machineId, object content)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdPaySuccess, content);
            return result;
        }

        public static CustomJsonResult QueryStatus(string operater, string appId, string merchId, string machineId, string messageId)
        {
            var result = new CustomJsonResult();
            var machine = BizFactory.Machine.GetOne(machineId);
            result = pushService.QueryStatus("", messageId);
            return result;
        }


    }
}
