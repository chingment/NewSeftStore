using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPushSdk
{
    public static class PushService
    {
        private static IPushService pushService = new JgPushService();

        public static CustomJsonResult Send(string regId, string cmd, object content)
        {
            var result = new CustomJsonResult();
            pushService.Send(regId, cmd, content);
            return result;
        }

        public static CustomJsonResult SendUpdateMachineStockSlots(string regId, object content)
        {
            var result = new CustomJsonResult();
            pushService.Send(regId, "update:StockSlots", content);
            return result;
        }

        public static CustomJsonResult SendUpdateMachineHomeBanners(string regId, object content)
        {
            var result = new CustomJsonResult();
            pushService.Send(regId, "update:HomeBanners", content);
            return result;
        }

        public static CustomJsonResult SendUpdateMachineHomeLogo(string regId, string logoImgUrl)
        {
            var result = new CustomJsonResult();
            var date = new { url = logoImgUrl };
            pushService.Send(regId, "update:HomeLogo", date);
            return result;
        }

    }
}
