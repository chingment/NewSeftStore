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
    public enum SourceType
    {
        Unknow = 0,
        Merch = 1000,
        WaitPay = 2000,
        Payed = 3000,
        Completed = 4000,
        Canceled = 5000
    }

    public class RedisMq4GlobalProvider : RedisMqObject<RedisMq4GlobalHandle>
    {
        protected override string MessageQueueKeyName { get { return "RedisMq4Global"; } }
        protected override bool IsTran { get { return false; } }

        public void PushPayTransResultNotify(string ticket, E_PayPartner payParner, E_PayTransLogNotifyFrom from, string content)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.PayTransResultNotify;
            obj.Ticket = ticket;
            obj.Content = (new PayTransResultNotifyModel { PayPartner = payParner, From = from, Content = content }).ToJsonString();
            this.Push(obj);
        }

        public void PushPayRefundResultNotify(string ticket, E_PayPartner payParner, E_PayTransLogNotifyFrom from, string payTransId, string payRefundId, string content)
        {
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.PayRefundResultNotify;
            obj.Ticket = ticket;
            obj.Content = (new PayRefundResultNotifyModel { PayPartner = payParner, From = from, PayTransId = payTransId, PayRefundId = payRefundId, Content = content }).ToJsonString();
            this.Push(obj);
        }

        public CustomJsonResult PushEventNotify(string operater, string appId, string trgerId, string eventCode, string eventRemark, object eventContent = null)
        {
            var content = new EventNotifyModel();
            content.AppId = appId;
            content.Operater = operater;
            content.TrgerId = trgerId;
            content.EventCode = eventCode;
            content.EventRemark = eventRemark;
            content.EventContent = eventContent;

            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.EventNotify;
            obj.Ticket = IdWorker.Build(IdType.NewGuid);
            obj.Content = content.ToJsonString();
            this.Push(obj);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }

        public CustomJsonResult PushOperateLog(string operater, string appId, string trgerId, string eventCode, string eventRemark, object eventData)
        {
            var content = new OperateLogModel();
            content.AppId = appId;
            content.Operater = operater;
            content.TrgerId = trgerId;
            content.EventCode = eventCode;
            content.EventRemark = eventRemark;
            content.EventData = eventData;

            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.OperateLog;
            obj.Ticket = IdWorker.Build(IdType.NewGuid);
            obj.Content = content.ToJsonString();
            this.Push(obj);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
