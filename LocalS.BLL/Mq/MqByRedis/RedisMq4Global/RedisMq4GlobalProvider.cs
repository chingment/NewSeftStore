using LocalS.BLL.Mq.MqMessageConentModel;
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

        public void PushStockOperate(StockOperateModel messageConent)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.StockOperate;
            obj.Content = messageConent;
            this.Push(obj);
        }

    }
}
