using LocalS.BLL.Biz;
using LocalS.DAL;
using LocalS.Entity;
using Lumos;
using Lumos.Redis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Mq.MqByRedis
{
    public class RedisMq4GlobalHandle
    {
        public readonly string TAG = "RedisMq4GlobalHandle";
        public MqMessageType Type { get; set; }
        public string Ticket { get; set; }
        public string Content { get; set; }

        private static readonly object lock_Handle = new object();
        public void Handle()
        {
            lock (lock_Handle)
            {
                LogUtil.SetTrackId(this.Ticket);

                LogUtil.Info(TAG, string.Format("消息队列处理[{0}]，开始", this.Ticket));

                if (this.Type == MqMessageType.Unknow)
                {
                    LogUtil.Info(TAG, string.Format("消息队列处理[{0}]，结束,未知消息类型", this.Ticket));
                    return;
                }

                if (this.Content == null)
                {
                    LogUtil.Info(TAG, string.Format("消息队列处理[{0}]，结束,内容为空", this.Ticket));
                    return;
                }

                LogUtil.Info(TAG, string.Format("消息队列处理[{0}]，正在处理，类型：{1}，内容：{2}", this.Ticket, this.Type, this.Content));

                try
                {
                    switch (this.Type)
                    {
                        case MqMessageType.PayTransResultNotify:
                            LogUtil.Info(TAG, "进入->PayTransResultNotify");
                            var m_PayTransResultNotify = this.Content.ToJsonObject<PayTransResultNotifyModel>();
                            BizFactory.Order.PayTransResultNotify(IdWorker.Build(IdType.EmptyGuid), m_PayTransResultNotify.PayPartner, m_PayTransResultNotify.From, m_PayTransResultNotify.Content);
                            break;
                        case MqMessageType.PayRefundResultNotify:
                            LogUtil.Info(TAG, "进入->PayRefundResultNotify");
                            var m_PayRefundResultNotify = this.Content.ToJsonObject<PayRefundResultNotifyModel>();
                            BizFactory.Order.PayRefundResultNotify(IdWorker.Build(IdType.EmptyGuid), m_PayRefundResultNotify.PayPartner, m_PayRefundResultNotify.From, m_PayRefundResultNotify.PayTransId, m_PayRefundResultNotify.PayRefundId, m_PayRefundResultNotify.Content);
                            break;
                        case MqMessageType.EventNotify:
                            LogUtil.Info(TAG, "进入->EventNotify");
                            var m_EventNotifyModel = this.Content.ToJsonObject<EventNotifyModel>();
                            BizFactory.Event.Handle(m_EventNotifyModel);
                            break;
                        case MqMessageType.OperateLog:
                            LogUtil.Info(TAG, "进入->OperateLog");
                            var m_OperateLog = this.Content.ToJsonObject<OperateLogModel>();
                            BizFactory.OperateLog.Handle(m_OperateLog);
                            break;
                        case MqMessageType.ErpReplenishPlanBuild:
                            LogUtil.Info(TAG, "进入->ErpReplenishPlanBuild");
                            var m_ReplenishPlanBuild = this.Content.ToJsonObject<ReplenishPlanBuildModel>();
                            BizFactory.Erp.HandleReplenishPlanBuild(m_ReplenishPlanBuild);
                            break;
                        default:
                            LogUtil.Info(TAG, string.Format("消息队列处理[{0}]，不能处理", this.Ticket));
                            break;
                    }

                    LogUtil.Info(TAG, string.Format("消息队列处理[{0}]，结束", this.Ticket));
                }
                catch (Exception ex)
                {
                    LogUtil.Error(TAG, string.Format("消息队列处理[{0}]，结束，发生异常", this.Ticket), ex);
                }
            }
        }
    }
}


