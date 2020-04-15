using LocalS.BLL.Biz;
using LocalS.Entity;
using Lumos;
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

        public CustomJsonResult PushEventNotify(string operater, string appId, string merchId, string storeId, string machineId, string eventCode, string eventRemark, object eventContent = null)
        {
            var content = new EventNotifyModel();
            content.AppId = appId;
            content.Operater = operater;
            content.MerchId = merchId;
            content.StoreId = storeId;
            content.MachineId = machineId;
            content.EventCode = eventCode;
            content.EventRemark = eventRemark;
            content.EventContent = eventContent;

            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.EventNotify;
            obj.Ticket = GuidUtil.New();
            obj.Content = content;
            this.Push(obj);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }

        //public CustomJsonResult PushEventNotify(string operater, string appId, string machineId, string eventCode, string eventRemark, object eventContent = null)
        //{
        //    return PushEventNotify(operater, appId, null, null, machineId, eventCode, eventRemark, eventContent);
        //}
    }
}
