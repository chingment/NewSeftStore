﻿using Lumos;
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

        public static CustomJsonResult Send(string regId,string cmd, object content)
        {
            var result = new CustomJsonResult();
            pushService.Send(regId, cmd, content);
            return result;
        }

        public static CustomJsonResult SendUpdateProductSkuStock(string regId,object content)
        {
            var result = new CustomJsonResult();
            pushService.Send(regId, "update:productSkuStock", content);
            return result;
        }

        public static CustomJsonResult SendUpdateHomeBanner(string regId, object content)
        {
            var result = new CustomJsonResult();
            pushService.Send(regId, "update:homeBanner", content);
            return result;
        }

    }
}
