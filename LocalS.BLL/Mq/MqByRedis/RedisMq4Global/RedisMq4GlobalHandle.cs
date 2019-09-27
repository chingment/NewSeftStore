using LocalS.DAL;
using Lumos;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Mq.MqByRedis
{
    public class RedisMq4GlobalHandle
    {
        public MqMessageType Type { get; set; }
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
                            case MqMessageType.OrderReserve:
                                OrderReserve();
                                break;
                            case MqMessageType.OrderPayCompleted:
                                OrderPayCompleted();
                                break;
                            case MqMessageType.OrderCancle:
                                OrderCancle();
                                break;
                        }

                        LogUtil.Info(string.Format("消息队列处理消息类型：{0},处理结束,内容：{1}", this.Type, this.Content.ToJsonString()));
                        Console.WriteLine(string.Format("消息队列处理消息类型：{0},处理结束,内容：{1}", this.Type, this.Content.ToJsonString()));
                    }
                    catch (Exception ex)
                    {
                        LogUtil.Info(string.Format("消息队列处理消息类型：{0},处理失败，内容：{1}", this.Type, this.Content.ToJsonString()));
                        Console.WriteLine(string.Format("消息队列处理消息类型：{0},处理失败，内容：{1}", this.Type, this.Content.ToJsonString()));
                    }
                }
            }
        }


        private void OrderReserve()
        {
            using (DbContext CurrentDb = new DbContext())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    // string orderId = "";
                    // var ore= CurrentDb.OrderDetailsChildSon.Where(m=>m.Id== orderId)
                }
            }
        }
        private void OrderPayCompleted()
        {
            using (DbContext CurrentDb = new DbContext())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                    //var orderDetailsChildSons=CurrentDb.OR
                    //var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == order.MerchId && m.PrdProductSkuId == item.PrdProductSkuId && m.SlotId == item.SlotId && m.RefId == item.SellChannelRefId && m.RefType == item.SellChannelRefType).FirstOrDefault();

                    //sellChannelStock.LockQuantity -= item.Quantity;
                    //sellChannelStock.SellQuantity += item.Quantity;
                    //sellChannelStock.Mender = operater;
                    //sellChannelStock.MendTime = DateTime.Now;

                    //var sellChannelStockLog = new SellChannelStockLog();
                    //sellChannelStockLog.Id = GuidUtil.New();
                    //sellChannelStockLog.MerchId = item.MerchId;
                    //sellChannelStockLog.RefId = item.SellChannelRefId;
                    //sellChannelStockLog.RefType = item.SellChannelRefType;
                    //sellChannelStockLog.SlotId = item.SlotId;
                    //sellChannelStockLog.PrdProductSkuId = item.PrdProductSkuId;
                    //sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                    //sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                    //sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                    //sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.Lock;
                    //sellChannelStockLog.ChangeQuantity = item.Quantity;
                    //sellChannelStockLog.Creator = operater;
                    //sellChannelStockLog.CreateTime = DateTime.Now;
                    //sellChannelStockLog.RemarkByDev = string.Format("取消订单，恢复库存：{0}", item.Quantity);
                    //CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                }
            }
        }

        private void OrderCancle()
        {
            using (DbContext CurrentDb = new DbContext())
            {
                using (TransactionScope ts = new TransactionScope())
                {
                }
            }
        }
    }
}


