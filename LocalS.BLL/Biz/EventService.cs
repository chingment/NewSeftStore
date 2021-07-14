using LocalS.BLL.Mq;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class EventService : BaseService
    {
        public void Handle(EventNotifyModel model)
        {
            switch (model.EventCode)
            {
                case EventCode.login:
                    var loginLogModel = model.EventContent.ToJsonObject<LoginLogModel>();
                    HandleByLogin(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, loginLogModel);
                    break;
                case EventCode.logout:
                    var logoutLogModel = model.EventContent.ToJsonObject<LoginLogModel>();
                    HandleByLogout(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, logoutLogModel);
                    break;
                case EventCode.device_status:
                    var deviceStatusModel = model.EventContent.ToJsonObject<DeviceEventByDeviceStatusModel>();
                    HandleByDeviceStatus(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, deviceStatusModel);
                    break;
                case EventCode.vending_pickup:
                    var devicePickupModel = model.EventContent.ToJsonObject<DeviceEventByPickupModel>();
                    HandleByPickup(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, devicePickupModel);
                    break;
                case EventCode.vending_pickup_test:
                    var devicePickupTestModel = model.EventContent.ToJsonObject<DeviceEventByPickupTestModel>();
                    HandleByPickupTest(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, devicePickupTestModel);
                    break;
            }
        }

        private void HandleByLogin(string operater, string appId, string trgerId, string eventCode, string eventRemark, LoginLogModel model)
        {
            var userLoginHis = new SysUserLoginHis();
            userLoginHis.Id = IdWorker.Build(IdType.NewGuid);
            userLoginHis.UserId = operater;
            userLoginHis.AppId = appId;
            userLoginHis.LoginAccount = model.LoginAccount;
            userLoginHis.LoginFun = model.LoginFun;
            userLoginHis.LoginWay = model.LoginWay;
            userLoginHis.Ip = model.LoginIp;
            userLoginHis.LoginTime = DateTime.Now;
            userLoginHis.Result = model.LoginResult;
            userLoginHis.Description = eventRemark;
            userLoginHis.Remark = model.Remark;
            userLoginHis.CreateTime = DateTime.Now;
            userLoginHis.Creator = operater;
            CurrentDb.SysUserLoginHis.Add(userLoginHis);
            CurrentDb.SaveChanges();

            if (appId == AppId.MERCH || appId == AppId.STORETERM || appId == AppId.WXMINPRAGROM)
            {
                MqFactory.Global.PushOperateLog(operater, appId, trgerId, EventCode.login, eventRemark, model);
            }
        }
        private void HandleByLogout(string operater, string appId, string trgerId, string eventCode, string eventRemark, LoginLogModel model)
        {
            var userLoginHis = new SysUserLoginHis();
            userLoginHis.Id = IdWorker.Build(IdType.NewGuid);
            userLoginHis.UserId = operater;
            userLoginHis.AppId = appId;
            userLoginHis.LoginAccount = model.LoginAccount;
            userLoginHis.LoginFun = model.LoginFun;
            userLoginHis.LoginWay = model.LoginWay;
            userLoginHis.Ip = model.LoginIp;
            userLoginHis.LoginTime = DateTime.Now;
            userLoginHis.Result = model.LoginResult;
            userLoginHis.Description = eventRemark;
            userLoginHis.Remark = model.Remark;
            userLoginHis.CreateTime = DateTime.Now;
            userLoginHis.Creator = operater;
            CurrentDb.SysUserLoginHis.Add(userLoginHis);
            CurrentDb.SaveChanges();


            if (appId == AppId.MERCH || appId == AppId.STORETERM || appId == AppId.WXMINPRAGROM)
            {
                MqFactory.Global.PushOperateLog(operater, appId, trgerId, EventCode.logout, eventRemark, model);
            }

        }
        private void HandleByDeviceStatus(string operater, string appId, string trgerId, string eventCode, string eventRemark, DeviceEventByDeviceStatusModel model)
        {
            LogUtil.Info(">>>>>HandleByDeviceStatus");

            string deviceId = trgerId;
            var device = CurrentDb.Device.Where(m => m.Id == trgerId).FirstOrDefault();

            if (device == null)
                return;

            string storeName = BizFactory.Merch.GetStoreName(device.CurUseMerchId, device.CurUseStoreId);
            string shopName = BizFactory.Merch.GetShopName(device.CurUseMerchId, device.CurUseShopId);
            string operaterUserName = BizFactory.Merch.GetOperaterUserName(device.CurUseMerchId, operater);

            device.LastRequestTime = DateTime.Now;

            bool isLog = false;
            switch (model.Status)
            {
                case "running":
                    eventRemark = "运行正常";

                    if (device.RunStatus != E_DeviceRunStatus.Running)
                    {
                        isLog = true;
                    }

                    device.RunStatus = E_DeviceRunStatus.Running;
                    break;
                case "setting":
                    eventRemark = "维护中";

                    if (device.RunStatus != E_DeviceRunStatus.Setting)
                    {
                        isLog = true;
                    }

                    device.RunStatus = E_DeviceRunStatus.Setting;
                    break;
                case "excepition":

                    eventRemark = "异常";

                    if (device.RunStatus != E_DeviceRunStatus.Excepition)
                    {
                        isLog = true;
                    }

                    device.RunStatus = E_DeviceRunStatus.Excepition;

                    break;
                default:
                    eventRemark = "未知状态";
                    break;
            }

            CurrentDb.SaveChanges();

            if (isLog)
            {
                MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, deviceId, EventCode.device_status, string.Format("店铺：{0}，门店：{1}，设备：{2}，{3}", storeName, shopName, deviceId, eventRemark), model);
            }
        }
        private void HandleByPickup(string operater, string appId, string trgerId, string eventCode, string eventRemark, DeviceEventByPickupModel model)
        {
            if (model == null)
                return;

            List<StockChangeRecordModel> s_StockChangeRecords = new List<StockChangeRecordModel>();
            Order d_Order;
            List<OrderSub> d_OrderSubs;
            Merch d_Merch;
            using (TransactionScope ts = new TransactionScope())
            {
                if (string.IsNullOrEmpty(model.SignId))
                {
                    model.SignId = IdWorker.Build(IdType.NewGuid);
                }

                var d_OrderPickupLog = CurrentDb.OrderPickupLog.Where(m => m.Id == model.SignId).FirstOrDefault();

                if (d_OrderPickupLog != null)
                    return;

                string deviceId = trgerId;
                var d_Device = CurrentDb.Device.Where(m => m.Id == deviceId).FirstOrDefault();

                if (d_Device == null)
                    return;

                string storeName = BizFactory.Merch.GetStoreName(d_Device.CurUseMerchId, d_Device.CurUseStoreId);
                string shopName = BizFactory.Merch.GetShopName(d_Device.CurUseMerchId, d_Device.CurUseShopId);
                string operaterUserName = BizFactory.Merch.GetOperaterUserName(d_Device.CurUseMerchId, operater);

                d_Device.LastRequestTime = DateTime.Now;

                StringBuilder remark = new StringBuilder("");

                string skuName = "";
                var r_Sku = CacheServiceFactory.Product.GetSkuInfo(d_Device.CurUseMerchId, model.SkuId);
                if (r_Sku != null)
                {
                    skuName = r_Sku.Name;
                }

                if (model.PickupStatus == E_OrderPickupStatus.SendPickupCmd)
                {
                    remark.Append("发送命令");
                }
                else if (model.PickupStatus == E_OrderPickupStatus.Taked)
                {
                    remark.Append(string.Format("取货完成，用时：{0}", model.PickupUseTime));
                }
                else if (model.PickupStatus == E_OrderPickupStatus.Exception)
                {
                    remark.Append(string.Format("发生异常，原因：{0}", model.Remark));
                }
                else
                {
                    remark.Append(string.Format("当前动作：{0}，状态：{1}", model.ActionName, model.ActionStatusName));
                }

                d_Order = CurrentDb.Order.Where(m => m.Id == model.OrderId).FirstOrDefault();
                d_OrderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == model.OrderId && m.ShopMode == E_ShopMode.Device && m.DeviceId == d_Device.Id).ToList();
                d_Merch = CurrentDb.Merch.Where(m => m.Id == d_Order.MerchId).FirstOrDefault();

                if (d_Order != null)
                {

                    if (d_Order.Status != E_OrderStatus.Completed && !d_Order.ExIsHappen)
                    {
                        //是否触发过取货
                        if (d_Order.PickupTrgTime == null)
                        {
                            d_Order.PickupIsTrg = true;
                            d_Order.PickupTrgTime = DateTime.Now;
                            d_Order.PickupFlowLastDesc = "商品取货，正在出货";
                            d_Order.PickupFlowLastTime = DateTime.Now;

                            int timoutM = d_Order.Quantity * 5;

                            Task4Factory.Tim2Global.Enter(Task4TimType.Order2CheckPickupTimeout, d_Order.Id, DateTime.Now.AddMinutes(timoutM), new Order2CheckPickupTimeoutModel { OrderId = d_Order.Id, DeviceId = d_Order.DeviceId });
                        }

                        d_OrderPickupLog = new OrderPickupLog();
                        d_OrderPickupLog.Id = model.SignId;
                        d_OrderPickupLog.OrderId = model.OrderId;
                        d_OrderPickupLog.MerchId = d_Order.MerchId;
                        d_OrderPickupLog.ShopMode = E_ShopMode.Device;
                        d_OrderPickupLog.StoreId = d_Order.StoreId;
                        d_OrderPickupLog.ShopId = d_Order.ShopId;
                        d_OrderPickupLog.DeviceId = d_Order.DeviceId;
                        d_OrderPickupLog.UniqueId = model.UniqueId;
                        d_OrderPickupLog.UniqueType = E_UniqueType.OrderSub;
                        d_OrderPickupLog.SkuId = model.SkuId;
                        d_OrderPickupLog.CabinetId = model.CabinetId;
                        d_OrderPickupLog.SlotId = model.SlotId;
                        d_OrderPickupLog.Status = model.PickupStatus;
                        d_OrderPickupLog.ActionId = model.ActionId;
                        d_OrderPickupLog.ActionName = model.ActionName;
                        d_OrderPickupLog.ActionStatusCode = model.ActionStatusCode;
                        d_OrderPickupLog.ActionStatusName = model.ActionStatusName;
                        d_OrderPickupLog.ImgId = model.ImgId;
                        d_OrderPickupLog.ImgId2 = model.ImgId2;
                        d_OrderPickupLog.ImgId3 = model.ImgId3;
                        d_OrderPickupLog.PickupUseTime = model.PickupUseTime;
                        d_OrderPickupLog.ActionRemark = remark.ToString();
                        d_OrderPickupLog.Remark = model.Remark;
                        d_OrderPickupLog.CreateTime = DateTime.Now;
                        d_OrderPickupLog.Creator = operater;
                        CurrentDb.OrderPickupLog.Add(d_OrderPickupLog);

                        if (model.PickupStatus == E_OrderPickupStatus.Exception)
                        {

                            d_Order.ExIsHappen = true;
                            d_Order.ExHappenTime = DateTime.Now;

                            List<ImgSet> exImgUrls = new List<ImgSet>();

                            if (!string.IsNullOrEmpty(model.ImgId))
                            {
                                exImgUrls.Add(new ImgSet { Url = BizFactory.Order.GetPickImgUrl(model.ImgId) });
                            }

                            if (!string.IsNullOrEmpty(model.ImgId2))
                            {
                                exImgUrls.Add(new ImgSet { Url = BizFactory.Order.GetPickImgUrl(model.ImgId2) });
                            }

                            if (exImgUrls.Count > 0)
                            {
                                d_Order.ExImgUrls = exImgUrls.ToJsonString();
                            }

                            d_Order.PickupFlowLastDesc = "取货动作发生异常";
                            d_Order.PickupFlowLastTime = DateTime.Now;

                            foreach (var d_OrderSub in d_OrderSubs)
                            {
                                if (d_OrderSub.PayStatus == E_PayStatus.PaySuccess)
                                {
                                    if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked
                                        && d_OrderSub.PickupStatus != E_OrderPickupStatus.Exception
                                        && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked
                                        && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        d_OrderSub.PickupStatus = E_OrderPickupStatus.Exception;
                                        d_OrderSub.ExPickupReason = "取货动作发生异常";
                                        d_OrderSub.ExPickupIsHappen = true;
                                        d_OrderSub.ExPickupHappenTime = DateTime.Now;
                                    }
                                }

                            }

                            d_Device.ExIsHas = true;
                            d_Device.ExReason = "取货动作发生异常";

                            Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, d_Order.Id);
                        }
                        else
                        {

                            foreach (var d_OrderSub in d_OrderSubs)
                            {

                                if (d_OrderSub.Id == model.UniqueId)
                                {
                                    d_OrderSub.PickupFlowLastDesc = model.ActionName + model.ActionStatusName;
                                    d_OrderSub.PickupFlowLastTime = DateTime.Now;

                                    if (model.PickupStatus == E_OrderPickupStatus.Taked)
                                    {
                                        if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                        {
                                            var resultOperateStock = BizFactory.ProductSku.OperateStockQuantity(operater, EventCode.order_sign_take, d_OrderSub.ShopMode, d_OrderSub.MerchId, d_OrderSub.StoreId, d_OrderSub.ShopId, d_OrderSub.DeviceId, d_OrderSub.CabinetId, d_OrderSub.SlotId, d_OrderSub.SkuId, 1);
                                            if (resultOperateStock.Result != ResultType.Success)
                                            {
                                                return;
                                            }

                                            s_StockChangeRecords.AddRange(resultOperateStock.Data.ChangeRecords);

                                        }

                                        if (d_OrderSub.PickupEndTime == null)
                                        {
                                            d_OrderSub.PickupEndTime = DateTime.Now;
                                        }

                                    }

                                    if (d_OrderSub.PickupStartTime == null)
                                    {
                                        d_OrderSub.PickupStartTime = DateTime.Now;
                                    }

                                    if (d_OrderSub.PickupStatus != E_OrderPickupStatus.Taked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignTaked && d_OrderSub.PickupStatus != E_OrderPickupStatus.ExPickupSignUnTaked)
                                    {
                                        d_OrderSub.PickupStatus = model.PickupStatus;
                                    }

                                    CurrentDb.SaveChanges();
                                }
                            }

                            var orderDetailsChildSonsCompeleteCount = d_OrderSubs.Where(m => m.PickupStatus == E_OrderPickupStatus.Taked).Count();
                            //判断全部订单都是已完成
                            if (orderDetailsChildSonsCompeleteCount == d_OrderSubs.Count)
                            {
                                d_Order.PickupFlowLastDesc = "全部商品出货完成";
                                d_Order.PickupFlowLastTime = DateTime.Now;
                                d_Order.Status = E_OrderStatus.Completed;
                                d_Order.CompletedTime = DateTime.Now;

                                Task4Factory.Tim2Global.Exit(Task4TimType.Order2CheckPickupTimeout, d_Order.Id);
                            }
                        }
                    }

                }

                CurrentDb.SaveChanges();
                ts.Complete();

                MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, deviceId, EventCode.vending_pickup, string.Format("店铺：{0}，门店：{1}，设备：{2}，{3}", storeName, shopName, d_Device.Id, remark.ToString()), new { Rop = model, StockChangeRecords = s_StockChangeRecords });

            }


            if (!string.IsNullOrEmpty(d_Order.NotifyUrl))
            {
                try
                {

                    LogUtil.Info("NotifyUrl:" + d_Order.NotifyUrl);
                    string[] skuIds = d_OrderSubs.Select(m => m.SkuId).ToArray();

                    var d_Stocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == d_Order.MerchId && m.StoreId == d_Order.StoreId && m.DeviceId == d_Order.DeviceId && skuIds.Contains(m.SkuId)).ToList();

                    List<object> sku_stocks = new List<object>();

                    var stock_Skus = (from u in d_Stocks select new { u.SkuId, u.IsOffSell }).Distinct();

                    foreach (var stock_Sku in stock_Skus)
                    {
                        Dictionary<string, object> dics = new Dictionary<string, object>();
                        dics.Add("sku_id", stock_Sku.SkuId);
                        var r_Sku2 = CacheServiceFactory.Product.GetSkuInfo(d_Order.MerchId, stock_Sku.SkuId);
                        dics.Add("sku_cum_code", r_Sku2.CumCode);

                        var sku_Stocks = d_Stocks.Where(m => m.SkuId == stock_Sku.SkuId);

                        int sumQuantity = sku_Stocks.Sum(m => m.SumQuantity);
                        int waitPayLockQuantity = sku_Stocks.Sum(m => m.WaitPayLockQuantity);
                        int waitPickupLockQuantity = sku_Stocks.Sum(m => m.WaitPickupLockQuantity);
                        int sellQuantity = sku_Stocks.Sum(m => m.SellQuantity);
                        int warnQuantity = sku_Stocks.Sum(m => m.WarnQuantity);
                        int holdQuantity = sku_Stocks.Sum(m => m.HoldQuantity);
                        int maxQuantity = sku_Stocks.Sum(m => m.MaxQuantity);

                        dics.Add("sum_quantity", sumQuantity);
                        dics.Add("lock_quantity", waitPayLockQuantity + waitPickupLockQuantity);
                        dics.Add("sell_quantity", sellQuantity);
                        dics.Add("warn_quantity", warnQuantity);
                        dics.Add("hold_quantity", holdQuantity);
                        dics.Add("max_quantity", maxQuantity);
                        dics.Add("is_off_sell", stock_Sku.IsOffSell);

                        List<object> slots = new List<object>();
                        foreach (var sku_Stock in sku_Stocks)
                        {
                            Dictionary<string, object> dic2s = new Dictionary<string, object>();

                            dic2s.Add("cabinet_id", sku_Stock.CabinetId);
                            dic2s.Add("slot_id", sku_Stock.SlotId);
                            dic2s.Add("sum_quantity", sku_Stock.SumQuantity);
                            dic2s.Add("lock_quantity", sku_Stock.WaitPayLockQuantity + sku_Stock.WaitPickupLockQuantity);
                            dic2s.Add("sell_quantity", sku_Stock.SellQuantity);
                            dic2s.Add("warn_quantity", sku_Stock.WarnQuantity);
                            dic2s.Add("hold_quantity", sku_Stock.HoldQuantity);
                            dic2s.Add("max_quantity", sku_Stock.MaxQuantity);

                            slots.Add(dic2s);
                        }

                        dics.Add("slots", slots);

                        sku_stocks.Add(dics);
                    }

                    var sku_Ships = new List<object>();

                    foreach (var item in d_OrderSubs)
                    {
                        sku_Ships.Add(new
                        {
                            unique_id = item.Id,
                            cabinet_id = item.CabinetId,
                            slot_id = item.SlotId,
                            sku_id = item.SkuId,
                            sku_cum_code = item.SkuCumCode,
                            status = item.PickupStatus,
                            tips = item.ExPickupReason,
                        });
                    }

                    string notify_url = d_Order.NotifyUrl;

                    Dictionary<string, Object> ret = new Dictionary<string, object>();
                    ret.Add("low_order_id", d_Order.CumId);
                    ret.Add("up_order_id", d_Order.Id);
                    ret.Add("business_type", "ship");
                    ret.Add("detail", new
                    {
                        is_trg = d_Order.PickupIsTrg,
                        sku_stocks = sku_stocks,
                        sku_ships = sku_Ships,
                    });

                    string data = ret.ToJsonString();
                    LogUtil.Info("sign.data:" + data);
                    long timespan = (long)(DateTime.Now - TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1))).TotalSeconds;
                    string sign = GetSign(d_Merch.Id, d_Merch.IotApiSecret, timespan, data);

                    HttpUtil http = new HttpUtil();
                    Dictionary<string, string> headers = new Dictionary<string, string>();

                    string authorization = string.Format("merch_id={0},timestamp={1},sign={2}", d_Merch.Id, timespan, sign);
                    LogUtil.Info("authorization:" + authorization);
                    headers.Add("Authorization", authorization);

                    var result_http = http.HttpPostJson(notify_url, data, headers);

                    if (!string.IsNullOrEmpty(result_http))
                    {
                        LogUtil.Info("result_http=>" + result_http);
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error("", ex);
                }

            }

        }

        public string GetSign(string merchId, string secret, long timespan, string data)
        {
            var sb = new StringBuilder();

            sb.Append(merchId);
            sb.Append(secret);
            sb.Append(timespan.ToString());
            sb.Append(data);

            var material = string.Concat(sb.ToString().OrderBy(c => c));

            var input = Encoding.UTF8.GetBytes(material);

            var hash = SHA256Managed.Create().ComputeHash(input);

            StringBuilder sb2 = new StringBuilder();
            foreach (byte b in hash)
                sb2.Append(b.ToString("x2"));

            string str = sb2.ToString();

            return str;
        }


        private void HandleByPickupTest(string operater, string appId, string trgerId, string eventCode, string eventRemark, DeviceEventByPickupTestModel model)
        {
            if (model == null)
                return;

            string deviceId = trgerId;
            var d_Device = CurrentDb.Device.Where(m => m.Id == deviceId).FirstOrDefault();

            if (d_Device == null)
                return;

            d_Device.LastRequestTime = DateTime.Now;
            CurrentDb.SaveChanges();

            string storeName = BizFactory.Merch.GetStoreName(d_Device.CurUseMerchId, d_Device.CurUseStoreId);
            string shopName = BizFactory.Merch.GetShopName(d_Device.CurUseMerchId, d_Device.CurUseShopId);
            string operaterUserName = BizFactory.Merch.GetOperaterUserName(d_Device.CurUseMerchId, operater);

            StringBuilder remark = new StringBuilder("");
            string skuName = "[测试]";

            var r_Sku = CacheServiceFactory.Product.GetSkuInfo(d_Device.CurUseMerchId, model.SkuId);

            if (r_Sku != null)
            {
                skuName += r_Sku.Name;
            }

            if (model.PickupStatus == E_OrderPickupStatus.SendPickupCmd)
            {
                remark.Append("发送命令");
            }
            else if (model.PickupStatus == E_OrderPickupStatus.Taked)
            {
                remark.Append(string.Format("取货完成，用时：{0}", model.PickupUseTime));
            }
            else if (model.PickupStatus == E_OrderPickupStatus.Exception)
            {
                remark.Append(string.Format("发生异常，原因：{0}", model.Remark));
            }
            else
            {
                remark.Append(string.Format("当前动作：{0}，状态：{1}", model.ActionName, model.ActionStatusName));
            }

            MqFactory.Global.PushOperateLog(operater, AppId.STORETERM, deviceId, EventCode.vending_pickup_test, string.Format("店铺：{0}，门店：{1}，设备：{2}，{3}", storeName, shopName, d_Device.Id, remark.ToString()), model);

        }
    }
}
