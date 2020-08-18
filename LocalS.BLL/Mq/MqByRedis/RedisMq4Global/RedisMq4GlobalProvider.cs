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

        public void PushPayTransResultNotify(string ticket, E_PayPartner payParner, E_PayTransLogNotifyFrom from, string content)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.PayTransResultNotify;
            obj.Ticket = ticket;
            obj.Content = new PayTransResultNotifyModel { PayPartner = payParner, From = from, Content = content };
            this.Push(obj);
        }

        public void PushPayRefundResultNotify(string ticket, E_PayPartner payParner, E_PayTransLogNotifyFrom from, string payTransId, string payRefundId, string content)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.PayRefundResultNotify;
            obj.Ticket = ticket;
            obj.Content = new PayRefundResultNotifyModel { PayPartner = payParner, From = from, PayTransId= payTransId, PayRefundId= payRefundId, Content = content };
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
            obj.Ticket = IdWorker.Build(IdType.NewGuid);
            obj.Content = content;
            this.Push(obj);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
