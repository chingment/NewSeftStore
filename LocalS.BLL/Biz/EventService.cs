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
                case EventCode.device_status:
                    var deviceStatusModel = model.EventContent.ToJsonObject<DeviceEventByDeviceStatusModel>();
                    HandleByDeviceStatus(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, deviceStatusModel);
                    break;
                case EventCode.vending_pickup:
                    var devicePickupModel = model.EventContent.ToJsonObject<DeviceEventByPickupModel>();
                    HandleByPickup(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, model.EventMsgId, model.EventMsgMode, devicePickupModel);
                    break;
                case EventCode.vending_pickup_test:
                    var devicePickupTestModel = model.EventContent.ToJsonObject<DeviceEventByPickupTestModel>();
                    HandleByPickupTest(model.Operater, model.AppId, model.TrgerId, model.EventCode, model.EventRemark, devicePickupTestModel);
                    break;
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
                case "exception":

                    eventRemark = "异常";

                    if (device.RunStatus != E_DeviceRunStatus.Exception)
                    {
                        isLog = true;
                    }

                    device.RunStatus = E_DeviceRunStatus.Exception;

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
        private void HandleByPickup(string operater, string appId, string trgerId, string eventCode, string eventRemark, int eventMsgId, string eventMsgMode, DeviceEventByPickupModel model)
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
                    d_OrderPickupLog.MsgId = eventMsgId;
                    d_OrderPickupLog.MsgMode=eventMsgMode;
                    d_OrderPickupLog.CreateTime = DateTime.Now;
                    d_OrderPickupLog.Creator = operater;
                    CurrentDb.OrderPickupLog.Add(d_OrderPickupLog);

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
                if (d_Order.Status == E_OrderStatus.Completed || d_Order.ExIsHappen == true)
                {
                    BizFactory.Order.NotifyMerchShip(operater, d_Order.MerchId, d_Order.Id);
                }
            }

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
