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
        private static IPushService pushService = new JgPushService();

        public static CustomJsonResult Send(string operater, string appId, string merchId, string machineId, string cmd, object content)
        {
            var result = new CustomJsonResult();

            var machine = BizFactory.Machine.GetOne(machineId);

            result = pushService.Send(machine.JPushRegId, cmd, content);

            if (result.Result == ResultType.Success)
            {
                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, machine.StoreId, machineId, cmd, "命令发送成功");
            }
            else
            {
                MqFactory.Global.PushEventNotify(operater, AppId.MERCH, merchId, machine.StoreId, machineId, cmd, "命令发送失败");
            }

            return result;
        }

        public static CustomJsonResult SendUpdateProductSkuStock(string operater, string appId, string merchId, string machineId, object content)
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

        public static CustomJsonResult SendRebootSys(string operater, string appId, string merchId, string machineId)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdRebootSys, "重启系统");
            return result;
        }

        public static CustomJsonResult SendShutdownSys(string operater, string appId, string merchId, string machineId)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdShutdownSys, "关闭系统");
            return result;
        }

        public static CustomJsonResult SendSetSysStatus(string operater, string appId, string merchId, string machineId, object content)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdSetSysStatus, content);
            return result;
        }

        public static CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string machineId, object content)
        {
            var result = new CustomJsonResult();
            result = Send(operater, appId, merchId, machineId, EventCode.MCmdPaySuccess, content);
            return result;
        }

    }
}
