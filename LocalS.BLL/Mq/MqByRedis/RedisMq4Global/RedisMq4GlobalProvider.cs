﻿using LocalS.BLL.Mq.MqMessageConentModel;
using LocalS.Entity;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq.MqByRedis
{
    public class RedisMq4GlobalProvider : RedisMqObject<RedisMq4GlobalHandle>
    {
        protected override string MessageQueueKeyName { get { return "RedisMq4Global"; } }
        protected override bool IsTran { get { return false; } }

        public void PushPayResultNotify(string ticket, E_OrderPayPartner payParner, E_OrderNotifyLogNotifyFrom from, string content)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.PayResultNotify;
            obj.Ticket = ticket;
            obj.Content = new PayResultNotifyModel { PayPartner = payParner, From = from, Content = content };
            this.Push(obj);
        }

        public void PushAccessLog(string ticket, string content)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.AccessLog;
            obj.Ticket = ticket;
            obj.Content = new AccessLogModel { };
            this.Push(obj);
        }
    }
}
