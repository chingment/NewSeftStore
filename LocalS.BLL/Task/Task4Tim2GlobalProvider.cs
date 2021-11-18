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
        Order2CheckReservePay = 1,
        Order2CheckPickupTimeout = 2,
        PayTrans2CheckStatus = 3,
        PayRefundCheckStatus = 4,
        DeviceRunInfo = 5,
        RentOrder2CheckExpire = 6
    }

    public class Task4Tim2GlobalProvider : BaseService, IJob
    {
        private static readonly string TAG = "Task4Tim2GlobalProvider";

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


        public void Exit(Task4TimType type, string[] ids)
        {
            foreach (var id in ids)
            {
                RedisManager.Db.HashDelete(key, id);
            }
        }

        public void Exit(Task4TimType type, string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

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
                LogUtil.Info(TAG, string.Format("共有{0}条记录需要检查状态", lists.Count));
                if (lists.Count > 0)
                {
                    foreach (var m in lists)
                    {
                        LogUtil.SetTrackId();

                        switch (m.Type)
                        {
                            case Task4TimType.Order2CheckReservePay:
                                #region 检查支付状态
                                LogUtil.Info(TAG, string.Format("开始执行订单查询,时间：{0}", DateTime.Now));
                                var order = m.Data.ToJsonObject<Order2CheckPayModel>();
                                LogUtil.Info(TAG, string.Format("查询订单号：{0}", order.Id));
                                //判断支付过期时间
                                if (m.ExpireTime.AddMinutes(1) < DateTime.Now)
                                {
                                    LogUtil.Info(TAG, string.Format("订单号：{0},订单支付有效时间过期", order.Id));
                                    BizFactory.Order.Cancle(IdWorker.Build(IdType.EmptyGuid), order.Id, null, E_OrderCancleType.PayTimeout, "订单支付有效时间过期");
                                }
                                LogUtil.Info(TAG, string.Format("结束执行订单查询,时间:{0}", DateTime.Now));
                                #endregion            
                                break;
                            case Task4TimType.PayTrans2CheckStatus:
                                #region 检查支付状态
                                LogUtil.Info(TAG, string.Format("开始执行交易查询,时间：{0}", DateTime.Now));
                                var payTrans = m.Data.ToJsonObject<PayTrans2CheckStatusModel>();
                                LogUtil.Info(TAG, string.Format("查询交易号：{0}", payTrans.Id));
                                //判断支付过期时间
                                if (m.ExpireTime.AddMinutes(1) >= DateTime.Now)
                                {
                                    //未过期查询支付状态
                                    string content = "";
                                    switch (payTrans.PayPartner)
                                    {
                                        case E_PayPartner.Wx:
                                            #region Wx
                                            switch (payTrans.PayCaller)
                                            {
                                                case E_PayCaller.WxByNt:
                                                    var wxByNt_AppInfoConfig = BizFactory.Store.GetWxMpAppInfoConfig(payTrans.StoreId);
                                                    content = SdkFactory.Wx.PayTransQuery(wxByNt_AppInfoConfig, payTrans.Id);
                                                    break;
                                                case E_PayCaller.WxByMp:
                                                    var wxByMp_AppInfoConfig = BizFactory.Store.GetWxMpAppInfoConfig(payTrans.StoreId);
                                                    content = SdkFactory.Wx.PayTransQuery(wxByMp_AppInfoConfig, payTrans.Id);
                                                    break;
                                            }
                                            #endregion
                                            break;
                                        case E_PayPartner.Zfb:
                                            #region Ali
                                            switch (payTrans.PayCaller)
                                            {
                                                case E_PayCaller.ZfbByNt:
                                                    var zfbByNt_AppInfoConfig = BizFactory.Store.GetZfbMpAppInfoConfig(payTrans.StoreId);
                                                    content = SdkFactory.Zfb.PayTransQuery(zfbByNt_AppInfoConfig, payTrans.Id);
                                                    break;
                                            }
                                            #endregion
                                            break;
                                        case E_PayPartner.Tg:
                                            #region Tg
                                            switch (payTrans.PayCaller)
                                            {
                                                case E_PayCaller.AggregatePayByNt:
                                                    var tgPay_AppInfoConfig = BizFactory.Store.GetTgPayInfoConfg(payTrans.StoreId);
                                                    content = SdkFactory.TgPay.PayTransQuery(tgPay_AppInfoConfig, payTrans.Id);
                                                    break;
                                            }
                                            #endregion Tg
                                            break;
                                        case E_PayPartner.Xrt:
                                            #region Xrt

                                            var xrtPay_AppInfoConfig = BizFactory.Store.GetXrtPayInfoConfg(payTrans.StoreId);
                                            content = SdkFactory.XrtPay.PayTransQuery(xrtPay_AppInfoConfig, payTrans.Id);

                                            #endregion
                                            break;
                                    }

                                    LogUtil.Info(TAG, string.Format("交易号：{0},查询支付结果文件:{1}", payTrans.Id, content));
                                    MqFactory.Global.PushPayTransResultNotify(IdWorker.Build(IdType.EmptyGuid), payTrans.PayPartner, E_PayTransLogNotifyFrom.Query, content);
                                }
                                else
                                {
                                    LogUtil.Info(TAG, string.Format("交易号：{0},订单支付有效时间过期", payTrans.Id));
                                    Task4Factory.Tim2Global.Exit(Task4TimType.PayTrans2CheckStatus, payTrans.Id);
                                    foreach (var orderId in payTrans.OrderIds)
                                    {
                                        BizFactory.Order.Cancle(IdWorker.Build(IdType.EmptyGuid), orderId, null, E_OrderCancleType.PayTimeout, "订单支付有效时间过期");
                                    }
                                }
                                LogUtil.Info(TAG, string.Format("结束执行交易查询,时间:{0}", DateTime.Now));
                                #endregion
                                break;
                            case Task4TimType.Order2CheckPickupTimeout:
                                #region 检查订单是否取货超时
                                var orderSub2CheckPickupTimeoutModel = m.Data.ToJsonObject<Order2CheckPickupTimeoutModel>();
                                if (m.ExpireTime.AddMinutes(1) <= DateTime.Now)
                                {
                                    Order2CheckPickupTimeout(orderSub2CheckPickupTimeoutModel);
                                }
                                #endregion
                                break;
                            case Task4TimType.PayRefundCheckStatus:
                                #region 检查订单退款是否成功
                                var payRefund = m.Data.ToJsonObject<PayRefund2CheckStatusModel>();
                                LogUtil.Info(TAG, string.Format("查询退款单号：{0}", payRefund.Id));
                                //判断支付过期时间
                                if (m.ExpireTime.AddMinutes(1) >= DateTime.Now)
                                {
                                    //未过期查询支付状态
                                    string content = "";
                                    switch (payRefund.PayPartner)
                                    {
                                        case E_PayPartner.Wx:
                                            #region Wx
                                            var wxByMp_AppInfoConfig = BizFactory.Store.GetWxMpAppInfoConfig(payRefund.MerchId);
                                            content = SdkFactory.Wx.PayRefundQuery(wxByMp_AppInfoConfig, payRefund.PayTransId, payRefund.Id);
                                            #endregion
                                            break;
                                        case E_PayPartner.Xrt:
                                            #region Wx
                                            var xrt_AppInfoConfig = BizFactory.Store.GetXrtPayInfoConfg(payRefund.StoreId);
                                            content = SdkFactory.XrtPay.PayRefundQuery(xrt_AppInfoConfig, payRefund.PayTransId, payRefund.Id);
                                            #endregion
                                            break;
                                    }

                                    LogUtil.Info(TAG, string.Format("退款单号：{0},查询结果文件:{1}", payRefund.Id, content));
                                    MqFactory.Global.PushPayRefundResultNotify(IdWorker.Build(IdType.EmptyGuid), payRefund.PayPartner, E_PayTransLogNotifyFrom.Query, content);
                                }
                                else
                                {
                                    LogUtil.Info(TAG, string.Format("退款单号：{0},有效时间过期", payRefund.Id));
                                    Task4Factory.Tim2Global.Exit(Task4TimType.PayRefundCheckStatus, payRefund.Id);
                                }
                                #endregion
                                break;
                            case Task4TimType.RentOrder2CheckExpire:
                                #region 检查租赁订单是否到期
                                var model5 = m.Data.ToJsonObject<RentOrder2CheckExpire>();
                                //判断支付过期时间
                                if (m.ExpireTime < DateTime.Now)
                                {
                                    LogUtil.Info("检查到过时订单:" + model5.POrderId);

                                    BizFactory.Order.NotifyClientExpire(IdWorker.Build(IdType.EmptyGuid), model5.ClientUserId, model5.SkuId, model5.SkuName, model5.ExpireDate, model5.POrderId);
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
                LogUtil.Error(TAG, "全局定时任务发生异常", ex);
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

        private void Order2CheckPickupTimeout(Order2CheckPickupTimeoutModel model)
        {
            using (TransactionScope ts = new TransactionScope())
            {
                var device = CurrentDb.Device.Where(m => m.Id == model.DeviceId).FirstOrDefault();
                var order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();
                var orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == model.OrderId && m.DeviceId == model.DeviceId).ToList();

                if (order != null)
                {
                    order.ExIsHappen = true;
                    order.ExHappenTime = DateTime.Now;
                    order.MendTime = DateTime.Now;
                    order.Mender = IdWorker.Build(IdType.EmptyGuid);
                }

                if (orderSubs.Count > 0)
                {
                    foreach (var orderSub in orderSubs)
                    {
                        if (orderSub.PickupStatus != E_OrderPickupStatus.Taked
                            && orderSub.PickupStatus != E_OrderPickupStatus.Exception
                            && orderSub.PickupStatus != E_OrderPickupStatus.UnTaked)
                        {
                            orderSub.PickupStatus = E_OrderPickupStatus.Exception;
                            orderSub.ExPickupIsHappen = true;
                            orderSub.ExPickupHappenTime = DateTime.Now;
                            orderSub.ExPickupReason = "后台检查，取货动作发生超时";
                            orderSub.MendTime = DateTime.Now;
                            orderSub.Mender = IdWorker.Build(IdType.EmptyGuid);
                        }
                    }
                }

                if (device != null)
                {
                    device.ExIsHas = true;
                    device.ExReason = "后台检查，取货动作发生超时";
                    device.MendTime = DateTime.Now;
                    device.Mender = IdWorker.Build(IdType.EmptyGuid);
                }

                CurrentDb.SaveChanges();
                ts.Complete();

                Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, order.Id);
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
                                string sql = "INSERT INTO SellChannelStockDateHis(Id, MerchId, StoreId, ShopMode, ShopId,DeviceId,CabinetId, SlotId, SpuId, SkuId, SellQuantity, WaitPayLockQuantity, WaitPickupLockQuantity,SumQuantity, SalePrice, IsOffSell, MaxQuantity,[Version],StockDate, Creator, CreateTime)select LOWER(REPLACE(LTRIM(NEWID()), '-', '')), MerchId, StoreId, ShopMode, ShopId,DeviceId, CabinetId, SlotId, SpuId, SkuId, SellQuantity, WaitPayLockQuantity, WaitPickupLockQuantity,SumQuantity, SalePrice, IsOffSell, MaxQuantity,[Version],'" + stockDate + "', '00000000000000000000000000000000', getdate()  from SellChannelStock where merchId='" + merch.Id + "' ";
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
