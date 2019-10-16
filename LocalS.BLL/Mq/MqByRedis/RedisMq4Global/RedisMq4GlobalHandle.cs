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
                                LogUtil.Info("StockOperate");
                                StockOperateModel t1 = Newtonsoft.Json.JsonConvert.DeserializeObject<StockOperateModel>(Newtonsoft.Json.JsonConvert.SerializeObject(this.Content));
                                StockOperate(GuidUtil.Empty(), t1);
                                break;
                            case MqMessageType.PayResultNotify:
                                LogUtil.Info("PayResultNotify");
                                PayResultNotifyModel t2 = Newtonsoft.Json.JsonConvert.DeserializeObject<PayResultNotifyModel>(Newtonsoft.Json.JsonConvert.SerializeObject(this.Content));
                                BLL.Biz.BizFactory.Order.PayResultNotify(GuidUtil.Empty(), t2.From, t2.Content);
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


        private void StockOperate(string operater, StockOperateModel model)
        {
            try
            {

                using (DbContext CurrentDb = new DbContext())
                {
                    using (TransactionScope ts = new TransactionScope())
                    {

                        #region 更新数据库 
                        switch (model.OperateType)
                        {
                            case StockOperateType.OrderReserveSuccess:

                                foreach (var stock in model.OperateStocks)
                                {
                                    var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == stock.MerchId && m.PrdProductSkuId == stock.ProductSkuId && m.SlotId == stock.SlotId && m.RefType == stock.RefType && m.RefId == stock.RefId).FirstOrDefault();

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
                                    sellChannelStockLog.PrdProductSkuId = stock.ProductSkuId;
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
                                    var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == stock.MerchId && m.PrdProductSkuId == stock.ProductSkuId && m.SlotId == stock.SlotId && m.RefType == stock.RefType && m.RefId == stock.RefId).FirstOrDefault();
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
                                    sellChannelStockLog.PrdProductSkuId = stock.ProductSkuId;
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
                                    var sellChannelStock = CurrentDb.SellChannelStock.Where(m => m.MerchId == stock.MerchId && m.PrdProductSkuId == stock.ProductSkuId && m.SlotId == stock.SlotId && m.RefType == stock.RefType && m.RefId == stock.RefId).FirstOrDefault();

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
                                    sellChannelStockLog.PrdProductSkuId = stock.ProductSkuId;
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
                        #endregion

                        #region 更新缓存

                        switch (model.OperateType)
                        {
                            case StockOperateType.OrderReserveSuccess:
                                //foreach (var stock in model.OperateStocks)
                                //{
                                //    CacheServiceFactory.ProductSku.OperateStock(stock.MerchId, stock.ProductSkuId, StockOperateType.OrderReserveSuccess, stock.RefType, stock.RefId, stock.SlotId, stock.Quantity);
                                //}
                                break;
                            case StockOperateType.OrderPaySuccess:

                                foreach (var stock in model.OperateStocks)
                                {
                                    CacheServiceFactory.ProductSku.OperateStock(stock.MerchId, stock.ProductSkuId, StockOperateType.OrderPaySuccess, stock.RefType, stock.RefId, stock.SlotId, stock.Quantity);
                                }

                                break;
                            case StockOperateType.OrderCancle:

                                foreach (var stock in model.OperateStocks)
                                {
                                    CacheServiceFactory.ProductSku.OperateStock(stock.MerchId, stock.ProductSkuId, StockOperateType.OrderCancle, stock.RefType, stock.RefId, stock.SlotId, stock.Quantity);
                                }

                                break;
                        }

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}


