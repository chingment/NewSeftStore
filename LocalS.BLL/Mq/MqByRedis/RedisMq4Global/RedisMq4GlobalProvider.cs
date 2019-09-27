using LocalS.BLL.Mq.MqMessageConentModel;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Mq.MqByRedis
{
    public class RedisMq4GlobalProvider : RedisMqObject<RedisMq4GlobalHandle>
    {
        protected override string MessageQueueKeyName { get { return "RedisMq4Global"; } }
        protected override bool IsTran { get { return false; } }

        public void PushOrderReserve(OrderReserveModel messageConent)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.OrderReserve;
            obj.Content = messageConent;
            this.Push(obj);
        }

        public void PushOrderCancle(OrderCancleModel messageConent)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.OrderCancle;
            obj.Content = messageConent;
            this.Push(obj);
        }

        public void PushOrderPayCompleted(OrderPayCompletedModel messageConent)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.OrderPayCompleted;
            obj.Content = messageConent;
            this.Push(obj);
        }
    }
}
