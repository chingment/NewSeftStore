using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace LocalS.BLL.Task
{

    public enum Task4TimType
    {
        Unknow = 0,
        Order2CheckPay = 1,
        Order2CheckPickupTimeout = 2
    }

    public class Task4Tim2GlobalProvider : BaseDbContext, IJob
    {
        private static readonly string key = "task4Tim2Global";

        public void Enter(Task4TimType type, string id, DateTime expireTime, object data)
        {
            var d = new TaskData();
            d.Id = id;
            d.Type = type;
            d.ExpireTime = expireTime;
            d.Data = data;
            RedisManager.Db.HashSetAsync(key, d.Id, d.ToJsonString(), StackExchange.Redis.When.Always);
        }

        public void UpdateData(Task4TimType type, string id, object data)
        {
            RedisManager.Db.HashSetAsync(key, id, data.ToJsonString(), StackExchange.Redis.When.Always);
        }

        public void Exit(Task4TimType type, string id)
        {
            RedisManager.Db.HashDelete(key, id);
        }

        public static List<TaskData> GetList()
        {
            List<TaskData> list = new List<TaskData>();
            var hs = RedisManager.Db.HashGetAll(key);

            var d = (from i in hs select i).ToList();

            foreach (var item in d)
            {
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<TaskData>(item.Value);
                list.Add(obj);
            }
            return list;
        }

        public void Execute(IJobExecutionContext context)
        {
            #region 检查支付状态
            try
            {
                var lists = GetList();
                LogUtil.Info(string.Format("共有{0}条记录需要检查状态", lists.Count));
                if (lists.Count > 0)
                {
                    foreach (var m in lists)
                    {
                        switch (m.Type)
                        {
                            case Task4TimType.Order2CheckPay:
                                LogUtil.Info(string.Format("开始执行订单查询,时间：{0}", DateTime.Now));
                                #region 检查支付状态
                                var order = m.Data.ToJsonObject<Order2CheckPayModel>();
                                LogUtil.Info(string.Format("查询订单号：{0}", order.Id));
                                //判断支付过期时间
                                if (m.ExpireTime.AddMinutes(1) >= DateTime.Now)
                                {
                                    //未过期查询支付状态
                                    string content = "";
                                    switch (order.PayPartner)
                                    {
                                        case E_OrderPayPartner.Wx:
                                            #region Wx
                                            switch (order.PayCaller)
                                            {
                                                case E_OrderPayCaller.WxByNt:
                                                    var wxByNt_AppInfoConfig = BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);
                                                    content = SdkFactory.Wx.PayQuery(wxByNt_AppInfoConfig, order.Id);
                                                    break;
                                                case E_OrderPayCaller.WxByMp:
                                                    var wxByMp_AppInfoConfig = BizFactory.Merch.GetWxMpAppInfoConfig(order.MerchId);
                                                    content = SdkFactory.Wx.PayQuery(wxByMp_AppInfoConfig, order.Id);
                                                    break;
                                            }
                                            #endregion
                                            break;
                                        case E_OrderPayPartner.Zfb:
                                            #region Ali
                                            switch (order.PayCaller)
                                            {
                                                case E_OrderPayCaller.ZfbByNt:
                                                    var zfbByNt_AppInfoConfig = BizFactory.Merch.GetZfbMpAppInfoConfig(order.MerchId);
                                                    content = SdkFactory.Zfb.PayQuery(zfbByNt_AppInfoConfig, order.Id);
                                                    break;
                                            }
                                            #endregion
                                            break;
                                        case E_OrderPayPartner.Tg:
                                            #region Tg
                                            switch (order.PayCaller)
                                            {
                                                case E_OrderPayCaller.AggregatePayByNt:
                                                    var tgPay_AppInfoConfig = BizFactory.Merch.GetTgPayInfoConfg(order.MerchId);
                                                    content = SdkFactory.TgPay.PayQuery(tgPay_AppInfoConfig, order.Id);
                                                    break;
                                            }
                                            #endregion Tg
                                            break;
                                        case E_OrderPayPartner.Xrt:
                                            #region Xrt

                                            var xrtPay_AppInfoConfig = BizFactory.Merch.GetXrtPayInfoConfg(order.MerchId);
                                            content = SdkFactory.XrtPay.PayQuery(xrtPay_AppInfoConfig, order.Id);

                                            #endregion
                                            break;
                                    }

                                    LogUtil.Info(string.Format("订单号：{0},查询支付结果文件:{1}", order.Id, content));
                                    MqFactory.Global.PushPayResultNotify(IdWorker.Build(IdType.EmptyGuid), order.PayPartner, E_OrderNotifyLogNotifyFrom.PayQuery, content);
                                }
                                else
                                {
                                    LogUtil.Info(string.Format("订单号：{0},订单支付有效时间过期", order.Id));
                                    BizFactory.Order.Cancle(IdWorker.Build(IdType.EmptyGuid), order.Id, E_OrderCancleType.PayTimeout, "订单支付有效时间过期");
                                }
                                #endregion
                                LogUtil.Info(string.Format("结束执行订单查询,时间:{0}", DateTime.Now));
                                break;
                            case Task4TimType.Order2CheckPickupTimeout:
                                #region 检查订单是否取货超时
                                var orderSub2CheckPickupTimeoutModel = m.Data.ToJsonObject<OrderSub2CheckPickupTimeoutModel>();
                                if (m.ExpireTime.AddMinutes(1) <= DateTime.Now)
                                {
                                    Order2CheckPickupTimeout(orderSub2CheckPickupTimeoutModel);
                                }
                                #endregion
                                break;

                        }
                    }
                }

                //定时生成库存报表
                BuildSellChannelStockDateReport();
            }
            catch (Exception ex)
            {
                LogUtil.Error("全局定时任务发生异常", ex);
            }
            #endregion
        }

        public class TaskData
        {
            public string Id { get; set; }
            public Task4TimType Type { get; set; }
            public DateTime ExpireTime { get; set; }
            public object Data { get; set; }
        }

        private void Order2CheckPickupTimeout(OrderSub2CheckPickupTimeoutModel model)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == model.MachineId).FirstOrDefault();
                var order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();
                var orderSub = CurrentDb.OrderSub.Where(m => m.OrderId == model.OrderId && m.SellChannelRefId == model.MachineId && m.SellChannelRefType == E_SellChannelRefType.Machine).FirstOrDefault();
                var orderSubChildUniques = CurrentDb.OrderSubChildUnique.Where(m => m.OrderSubId == orderSub.Id).ToList();

                if (orderSub != null)
                {
                    orderSub.ExIsHappen = true;
                    orderSub.ExHappenTime = DateTime.Now;
                }

                if (orderSubChildUniques.Count > 0)
                {
                    foreach (var orderSubChildUnique in orderSubChildUniques)
                    {
                        if (orderSubChildUnique.PickupStatus != E_OrderPickupStatus.Taked
                            && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.Exception
                            && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked
                            && orderSubChildUnique.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                        {

                            orderSubChildUnique.PickupStatus = E_OrderPickupStatus.Exception;
                            orderSubChildUnique.ExPickupIsHappen = true;
                            orderSubChildUnique.ExPickupHappenTime = DateTime.Now;
                        }
                    }
                }

                if (order != null)
                {
                    order.ExIsHappen = true;
                    order.ExHappenTime = DateTime.Now;
                }

                //if (machine != null)
                //{
                //    machine.ExIsHas = true;
                //}

                CurrentDb.SaveChanges();
                ts.Complete();

                Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, orderSub.Id);
            }
        }

        public void BuildSellChannelStockDateReport()
        {
            try
            {
                var merchs = CurrentDb.Merch.ToList();
                string stockDate = DateTime.Now.ToString("yyyy-MM-dd");

                List<SellChannelStockDateHis> sellChannelStockDateHiss = new List<SellChannelStockDateHis>();

                foreach (var merch in merchs)
                {

                    DateTime? buildStockRptDate = null;
                    try
                    {
                        buildStockRptDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + merch.BuildStockRptDate);
                    }
                    catch (Exception ex)
                    {

                    }

                    if (buildStockRptDate != null)
                    {
                        if (DateTime.Now > buildStockRptDate)
                        {
                            var hisRecordCount = CurrentDb.SellChannelStockDateHis.Where(m => m.MerchId == merch.Id && m.StockDate == stockDate).Count();
                            if (hisRecordCount == 0)
                            {
                                string sql = "INSERT INTO SellChannelStockDateHis(Id, MerchId, StoreId, SellChannelRefType, SellChannelRefId,CabinetId, SlotId, PrdProductId, PrdProductSkuId, SellQuantity, WaitPayLockQuantity, WaitPickupLockQuantity,SumQuantity, SalePrice, SalePriceByVip, IsOffSell, MaxQuantity,[Version],StockDate, Creator, CreateTime)select LOWER(REPLACE(LTRIM(NEWID()), '-', '')), MerchId, StoreId, SellChannelRefType, SellChannelRefId,CabinetId, SlotId, PrdProductId, PrdProductSkuId, SellQuantity, WaitPayLockQuantity, WaitPickupLockQuantity,SumQuantity, SalePrice, SalePriceByVip, IsOffSell, MaxQuantity,[Version],'" + stockDate + "', '00000000000000000000000000000000', getdate()  from SellChannelStock where merchId='" + merch.Id + "' ";
                                int rows = DatabaseFactory.GetIDBOptionBySql().ExecuteSql(sql);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error("处理生成日库存报表失败", ex);

            }
        }
    }
}
