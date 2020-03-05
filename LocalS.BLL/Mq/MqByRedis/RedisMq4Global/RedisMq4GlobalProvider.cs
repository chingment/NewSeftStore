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

        public void PushOperateLog(string appId, string operater, string machineId, string action, string remark, object parms = null)
        {
            var content = new OperateLogModel();
            content.AppId = appId;
            content.Operater = operater;
            content.Action = action;
            content.Remark = remark;
            content.MachineId = machineId;
            content.Parms = parms;

            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.OperateLog;
            obj.Ticket = GuidUtil.New();
            obj.Content = content;
            this.Push(obj);
        }

        public void PushMachineEventNotify(string operater,string appId, string machineId, double lat, double lng, E_MachineEventType type, object content)
        {
            var _content = new MachineEventNotifyModel();
            _content.AppId = appId;
            _content.MachineId = machineId;
            _content.Type = type;
            _content.Content = content;
            _content.Operater = operater;
            var obj = new RedisMq4GlobalHandle();
            obj.Type = MqMessageType.MachineEventNotify;
            obj.Ticket = GuidUtil.New();
            obj.Content = _content;
            this.Push(obj);
        }
    }
}
