using LocalS.DAL;
using LocalS.Entity;
using Lumos;
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
        public MqMessageType Type { get; set; }
        public string Ticket { get; set; }
        public object Content { get; set; }
        private static readonly object lock_Handle = new object();
        public void Handle()
        {
            lock (lock_Handle)
            {
                Console.WriteLine("消息队列处理");
                Console.WriteLine(string.Format("消息队列处理消息类型：{0},正在处理,内容：{1}", this.Type, this.Content.ToJsonString()));

                if (this.Type != MqMessageType.Unknow && this.Content != null)
                {
                    LogUtil.Info(string.Format("消息队列处理消息类型：{0},正在处理,内容：{1}", this.Type, this.Content.ToJsonString()));
                    Console.WriteLine(string.Format("消息队列处理消息类型：{0},正在处理,内容：{1}", this.Type, this.Content.ToJsonString()));
                    try
                    {
                        switch (this.Type)
                        {
                            case MqMessageType.PayResultNotify:
                                LogUtil.Info("PayResultNotify");
                                var payResultNotifyModel = Newtonsoft.Json.JsonConvert.DeserializeObject<PayResultNotifyModel>(Newtonsoft.Json.JsonConvert.SerializeObject(this.Content));
                                BLL.Biz.BizFactory.Order.PayResultNotify(GuidUtil.Empty(), payResultNotifyModel.PayPartner, payResultNotifyModel.From, payResultNotifyModel.Content);
                                break;
                            case MqMessageType.EventNotify:
                                LogUtil.Info("EventNotify");
                                var eventNotifyModel = Newtonsoft.Json.JsonConvert.DeserializeObject<EventNotifyModel>(Newtonsoft.Json.JsonConvert.SerializeObject(this.Content));
                                BLL.Biz.BizFactory.OperateLog.EventNotifyHandle(eventNotifyModel);
                                break;
                        }

                        LogUtil.Info(string.Format("消息队列处理消息类型：{0},处理结束,内容：{1}", this.Type, this.Content.ToJsonString()));
                        Console.WriteLine(string.Format("消息队列处理消息类型：{0},处理结束,内容：{1}", this.Type, this.Content.ToJsonString()));
                    }
                    catch (Exception ex)
                    {
                        LogUtil.Error("处理异常", ex);
                        LogUtil.Info(string.Format("消息队列处理消息类型：{0},处理失败，内容：{1}", this.Type, this.Content.ToJsonString()));
                        Console.WriteLine(string.Format("消息队列处理消息类型：{0},处理失败，内容：{1}", this.Type, this.Content.ToJsonString()));
                    }
                }
            }
        }
    }
}


