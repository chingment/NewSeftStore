using LocalS.BLL.Mq.MqMessageConentModel;
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

        public void PushPayResultNotify(string ticket, PayResultNotifyModel messageConent)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.PayResultNotify;
            obj.Ticket = ticket;
            obj.Content = messageConent;
            this.Push(obj);
        }

        public void PushOrderReserve(string ticket, LocalS.BLL.Biz.RopOrderReserve messageConent)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.OrderReserve;
            obj.Ticket = ticket;
            obj.Content = messageConent;
            this.Push(obj);
        }
    }
}
