using LocalS.BLL.Mq.MqMessageConentModel;
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
                            case MqMessageType.StockOperate:
                                StockOperate(GuidUtil.Empty(), (StockOperateModel)this.Content);
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


        private void StockOperate(string operater, StockOperateModel model)
        {
            using (DbContext CurrentDb = new DbContext())
            {
                using (TransactionScope ts = new TransactionScope())
                {

                    switch (model.OperateType)
                    {
                        case StockOperateType.OrderReserveSuccess:

                            foreach (var stock in model.OperateStocks)
                            {
                                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == stock.MerchId && m.PrdProductSkuId == stock.PrdProductSkuId && m.SlotId == stock.SlotId && m.RefType == stock.RefType && m.RefId == stock.RefId).FirstOrDefault();

                                sellChannelStock.LockQuantity += stock.Quantity;
                                sellChannelStock.SellQuantity -= stock.Quantity;
                                sellChannelStock.Mender = operater;
                                sellChannelStock.MendTime = DateTime.Now;

                                var sellChannelStockLog = new SellChannelStockLog();
                                sellChannelStockLog.Id = GuidUtil.New();
                                sellChannelStockLog.MerchId = stock.MerchId;
                                sellChannelStockLog.RefType = stock.RefType;
                                sellChannelStockLog.RefId = stock.RefId;
                                sellChannelStockLog.SlotId = stock.SlotId;
                                sellChannelStockLog.PrdProductSkuId = stock.PrdProductSkuId;
                                sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                                sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                                sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                                sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.ReserveSuccess;
                                sellChannelStockLog.ChangeQuantity = stock.Quantity;
                                sellChannelStockLog.Creator = operater;
                                sellChannelStockLog.CreateTime = DateTime.Now;
                                sellChannelStockLog.RemarkByDev = string.Format("预定成功，减少可销库存：{0}", stock.Quantity);
                                CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                            }
                            break;
                        case StockOperateType.OrderPaySuccess:

                            foreach (var stock in model.OperateStocks)
                            {
                                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == stock.MerchId && m.PrdProductSkuId == stock.PrdProductSkuId && m.SlotId == stock.SlotId && m.RefType == stock.RefType && m.RefId == stock.RefId).FirstOrDefault();
                                sellChannelStock.LockQuantity -= stock.Quantity;
                                sellChannelStock.SumQuantity -= stock.Quantity;
                                sellChannelStock.Mender = operater;
                                sellChannelStock.MendTime = DateTime.Now;

                                var sellChannelStockLog = new SellChannelStockLog();
                                sellChannelStockLog.Id = GuidUtil.New();
                                sellChannelStockLog.MerchId = stock.MerchId;
                                sellChannelStockLog.RefId = stock.RefId;
                                sellChannelStockLog.RefType = stock.RefType;
                                sellChannelStockLog.SlotId = stock.SlotId;
                                sellChannelStockLog.PrdProductSkuId = stock.PrdProductSkuId;
                                sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                                sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                                sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                                sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderPaySuccess;
                                sellChannelStockLog.ChangeQuantity = stock.Quantity;
                                sellChannelStockLog.Creator = operater;
                                sellChannelStockLog.CreateTime = DateTime.Now;
                                sellChannelStockLog.RemarkByDev = string.Format("成功支付，减少实际库存：{0}", stock.Quantity);
                                CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                            }

                            break;
                        case StockOperateType.OrderCancle:

                            foreach (var stock in model.OperateStocks)
                            {
                                var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == stock.MerchId && m.PrdProductSkuId == stock.PrdProductSkuId && m.SlotId == stock.SlotId && m.RefType == stock.RefType && m.RefId == stock.RefId).FirstOrDefault();

                                sellChannelStock.LockQuantity -= stock.Quantity;
                                sellChannelStock.SellQuantity += stock.Quantity;
                                sellChannelStock.Mender = operater;
                                sellChannelStock.MendTime = DateTime.Now;

                                var sellChannelStockLog = new SellChannelStockLog();
                                sellChannelStockLog.Id = GuidUtil.New();
                                sellChannelStockLog.MerchId = stock.MerchId;
                                sellChannelStockLog.RefId = stock.RefId;
                                sellChannelStockLog.RefType = stock.RefType;
                                sellChannelStockLog.SlotId = stock.SlotId;
                                sellChannelStockLog.PrdProductSkuId = stock.PrdProductSkuId;
                                sellChannelStockLog.SumQuantity = sellChannelStock.SumQuantity;
                                sellChannelStockLog.LockQuantity = sellChannelStock.LockQuantity;
                                sellChannelStockLog.SellQuantity = sellChannelStock.SellQuantity;
                                sellChannelStockLog.ChangeType = E_SellChannelStockLogChangeTpye.OrderCancle;
                                sellChannelStockLog.ChangeQuantity = stock.Quantity;
                                sellChannelStockLog.Creator = operater;
                                sellChannelStockLog.CreateTime = DateTime.Now;
                                sellChannelStockLog.RemarkByDev = string.Format("取消订单，恢复可销库存：{0}", stock.Quantity);
                                CurrentDb.SellChannelStockLog.Add(sellChannelStockLog);
                            }

                            break;
                    }

                    CurrentDb.SaveChanges();
                    ts.Complete();
                }
            }
        }
    }
}


