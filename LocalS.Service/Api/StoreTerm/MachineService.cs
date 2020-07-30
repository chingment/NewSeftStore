using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.Mq;
using LocalS.Entity;
using Lumos;
using Lumos.DbRelay;
using Lumos.Redis;
using Lumos.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.Service.Api.StoreTerm
{
    public class MachineService : BaseDbContext
    {
        public CustomJsonResult InitData(RopMachineInitData rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetMachineInitData();

            if (string.IsNullOrEmpty(rop.DeviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备编码为空");
            }

            var machine = CurrentDb.Machine.Where(m => m.ImeiId == rop.DeviceId || m.MacAddress == rop.DeviceId).FirstOrDefault();

            if (machine == null)
            {
                machine = new Machine();
                machine.Id = IdWorker.Build(IdType.MachineId);
                machine.Name = "贩卖X1";//默认名称
                machine.JPushRegId = rop.JPushRegId;
                machine.DeviceId = rop.DeviceId;
                machine.ImeiId = rop.ImeiId == null ? IdWorker.Build(IdType.NewGuid) : rop.ImeiId;
                machine.MacAddress = rop.MacAddress == null ? IdWorker.Build(IdType.NewGuid) : rop.MacAddress;
                machine.MainImgUrl = "http://file.17fanju.com/Upload/machine1.jpg";
                machine.AppVersionCode = rop.AppVersionCode;
                machine.AppVersionName = rop.AppVersionName;
                machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                machine.KindIsHidden = false;
                machine.KindRowCellSize = 3;
                machine.CreateTime = DateTime.Now;
                machine.Creator = IdWorker.Build(IdType.EmptyGuid);
                CurrentDb.Machine.Add(machine);
                CurrentDb.SaveChanges();
            }
            else
            {
                machine.JPushRegId = rop.JPushRegId;
                machine.AppVersionCode = rop.AppVersionCode;
                machine.AppVersionName = rop.AppVersionName;
                machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                machine.MendTime = DateTime.Now;
                machine.Mender = IdWorker.Build(IdType.EmptyGuid);
                CurrentDb.SaveChanges();
            }

            if (string.IsNullOrEmpty(machine.CurUseMerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(machine.CurUseStoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户店铺");
            }

            var machineInfo = BizFactory.Machine.GetOne(machine.Id);
            ret.Machine.Id = machineInfo.Id;
            ret.Machine.DeviceId = machineInfo.DeviceId;
            ret.Machine.Name = machineInfo.Name;
            ret.Machine.LogoImgUrl = machineInfo.LogoImgUrl;
            ret.Machine.MerchName = machineInfo.MerchName;
            ret.Machine.StoreName = machineInfo.StoreName;
            ret.Machine.CsrQrCode = machineInfo.CsrQrCode;
            ret.Machine.CsrPhoneNumber = machineInfo.CsrPhoneNumber;
            ret.Machine.CsrHelpTip = machineInfo.CsrHelpTip;
            ret.Machine.Cabinets = machineInfo.Cabinets;
            ret.Machine.IsHiddenKind = machineInfo.KindIsHidden;
            ret.Machine.KindRowCellSize = machineInfo.KindRowCellSize;
            ret.Machine.PayOptions = machineInfo.PayOptions;
            ret.Machine.CameraByChkIsUse = machine.CameraByChkIsUse;
            ret.Machine.CameraByJgIsUse = machine.CameraByJgIsUse;
            ret.Machine.CameraByRlIsUse = machine.CameraByRlIsUse;
            ret.Machine.MaxBuyNumber = 10;
            ret.Machine.ExIsHas = machineInfo.ExIsHas;
            ret.Machine.OstVern = machineInfo.OstVern;
            ret.Machine.MstVern = machineInfo.MstVern;
            ret.Machine.Scanner = machineInfo.Scanner;
            ret.Machine.FingerVeinner = machineInfo.FingerVeinner;
            ret.Machine.ImIsUse = machineInfo.ImIsUse;
            ret.Machine.ImPartner = machineInfo.ImPartner;
            ret.Machine.ImUserName = machineInfo.ImUserName;
            ret.Machine.ImPassword = machineInfo.ImPassword;

            ret.Banners = BizFactory.Machine.GetHomeBanners(machineInfo.Id);
            ret.ProductKinds = StoreTermServiceFactory.Machine.GetProductKinds(machineInfo.MerchId, machineInfo.StoreId, machineInfo.Id);
            ret.ProductSkus = StoreTermServiceFactory.Machine.GetProductSkus(machineInfo.MerchId, machineInfo.StoreId, machineInfo.Id);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public Dictionary<string, ProductSkuModel> GetProductSkus(string merchId, string storeId, string machineId)
        {
            var products = StoreTermServiceFactory.ProductSku.GetPageList(0, int.MaxValue, merchId, storeId, machineId);

            var dics = new Dictionary<string, ProductSkuModel>();

            if (products == null)
            {
                return dics;
            }

            if (products.Items == null)
            {
                return dics;
            }

            if (products.Items.Count == 0)
            {
                return dics;
            }

            foreach (var item in products.Items)
            {
                dics.Add(item.Id, item);
            }

            return dics;
        }

        public List<ProductKindModel> GetProductKinds(string merchId, string storeId, string machineId)
        {
            var productKindModels = new List<ProductKindModel>();

            var prdKinds = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();
            var sellChannelStocks = CurrentDb.SellChannelStock.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.SellChannelRefId == machineId).ToList();


            var prdKindModelByAll = new ProductKindModel();
            prdKindModelByAll.Id = IdWorker.Build(IdType.EmptyGuid);
            prdKindModelByAll.Name = "全部";
            prdKindModelByAll.Childs = sellChannelStocks.Select(m => m.PrdProductSkuId).Distinct().ToList();
            productKindModels.Add(prdKindModelByAll);

            foreach (var prdKind in prdKinds)
            {
                var prdKindModel = new ProductKindModel();
                prdKindModel.Id = prdKind.Id;
                prdKindModel.Name = prdKind.Name;

                var productIds = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.StoreKindId == prdKind.Id).Select(m => m.PrdProductId).Distinct().ToList();
                if (productIds.Count > 0)
                {
                    var productSkuIds = sellChannelStocks.Where(m => productIds.Contains(m.PrdProductId)).Select(m => m.PrdProductSkuId).Distinct().ToList();
                    if (productSkuIds.Count > 0)
                    {
                        prdKindModel.Childs = productSkuIds;
                        productKindModels.Add(prdKindModel);
                    }
                }
            }

            return productKindModels;
        }

        public CustomJsonResult CheckUpdate(RupMachineCheckUpdate rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var appSoftware = CurrentDb.AppSoftware.Where(m => m.AppId == rup.AppId && m.AppApiKey == rup.AppKey).FirstOrDefault();
            if (appSoftware == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            if (string.IsNullOrEmpty(appSoftware.VersionName))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            if (string.IsNullOrEmpty(appSoftware.ApkDownloadUrl))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            var model = new { versionCode = appSoftware.VersionCode, versionName = appSoftware.VersionName, apkDownloadUrl = appSoftware.ApkDownloadUrl };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", model);
        }

        public Task<bool> UpLoadTraceLog(RopAppTraceLog rop)
        {
            var task = Task.Run(() =>
            {
                if (rop.events != null)
                {
                    foreach (var pa in rop.appActions)
                    {
                        var appTraceLog = new AppTraceLog();
                        appTraceLog.Id = IdWorker.Build(IdType.NewGuid);
                        appTraceLog.AppTraceType = E_AppTraceType.Action;
                        appTraceLog.AppId = rop.device.appinfo.appId;
                        appTraceLog.AppVersion = rop.device.appinfo.appVersion;
                        appTraceLog.AppChannel = rop.device.appinfo.appChannel;
                        appTraceLog.DeviceDensity = rop.device.deviceinfo.deviceDensity;
                        appTraceLog.DeviceId = rop.device.deviceinfo.deviceId;
                        appTraceLog.DeviceLocale = rop.device.deviceinfo.deviceLocale;
                        appTraceLog.DeviceMacAddr = rop.device.deviceinfo.deviceMacAddr;
                        appTraceLog.DeviceModel = rop.device.deviceinfo.deviceModel;
                        appTraceLog.DeviceOsVersion = rop.device.deviceinfo.deviceOsVersion;
                        appTraceLog.DevicePlatform = rop.device.deviceinfo.devicePlatform;
                        appTraceLog.DeviceScreen = rop.device.deviceinfo.deviceScreen;
                        appTraceLog.IpAddr = rop.device.networkinfo.ipAddr;
                        appTraceLog.Wifi = rop.device.networkinfo.wifi;

                        appTraceLog.AppActionTime = pa.action_time;
                        appTraceLog.AppActionType = pa.action_type;
                        appTraceLog.AppActionDesc = pa.action_desc;
                        appTraceLog.CreateTime = DateTime.Now;
                        appTraceLog.Creator = IdWorker.Build(IdType.EmptyGuid);

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }

                    foreach (var pa in rop.pages)
                    {
                        var appTraceLog = new AppTraceLog();
                        appTraceLog.Id = IdWorker.Build(IdType.NewGuid);
                        appTraceLog.AppTraceType = E_AppTraceType.Page;
                        appTraceLog.AppId = rop.device.appinfo.appId;
                        appTraceLog.AppVersion = rop.device.appinfo.appVersion;
                        appTraceLog.AppChannel = rop.device.appinfo.appChannel;
                        appTraceLog.DeviceDensity = rop.device.deviceinfo.deviceDensity;
                        appTraceLog.DeviceId = rop.device.deviceinfo.deviceId;
                        appTraceLog.DeviceLocale = rop.device.deviceinfo.deviceLocale;
                        appTraceLog.DeviceMacAddr = rop.device.deviceinfo.deviceMacAddr;
                        appTraceLog.DeviceModel = rop.device.deviceinfo.deviceModel;
                        appTraceLog.DeviceOsVersion = rop.device.deviceinfo.deviceOsVersion;
                        appTraceLog.DevicePlatform = rop.device.deviceinfo.devicePlatform;
                        appTraceLog.DeviceScreen = rop.device.deviceinfo.deviceScreen;
                        appTraceLog.IpAddr = rop.device.networkinfo.ipAddr;
                        appTraceLog.Wifi = rop.device.networkinfo.wifi;

                        appTraceLog.PageId = pa.page_id;
                        appTraceLog.PageRefererPageId = pa.referer_page_id;
                        appTraceLog.PageStartTime = pa.page_start_time;
                        appTraceLog.PageEndTime = pa.page_end_time;

                        appTraceLog.CreateTime = DateTime.Now;
                        appTraceLog.Creator = IdWorker.Build(IdType.EmptyGuid);

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }

                    foreach (var ev in rop.events)
                    {
                        var appTraceLog = new AppTraceLog();
                        appTraceLog.Id = IdWorker.Build(IdType.NewGuid);
                        appTraceLog.AppTraceType = E_AppTraceType.Event;
                        appTraceLog.AppId = rop.device.appinfo.appId;
                        appTraceLog.AppVersion = rop.device.appinfo.appVersion;
                        appTraceLog.AppChannel = rop.device.appinfo.appChannel;
                        appTraceLog.DeviceDensity = rop.device.deviceinfo.deviceDensity;
                        appTraceLog.DeviceId = rop.device.deviceinfo.deviceId;
                        appTraceLog.DeviceLocale = rop.device.deviceinfo.deviceLocale;
                        appTraceLog.DeviceMacAddr = rop.device.deviceinfo.deviceMacAddr;
                        appTraceLog.DeviceModel = rop.device.deviceinfo.deviceModel;
                        appTraceLog.DeviceOsVersion = rop.device.deviceinfo.deviceOsVersion;
                        appTraceLog.DevicePlatform = rop.device.deviceinfo.devicePlatform;
                        appTraceLog.DeviceScreen = rop.device.deviceinfo.deviceScreen;
                        appTraceLog.IpAddr = rop.device.networkinfo.ipAddr;
                        appTraceLog.Wifi = rop.device.networkinfo.wifi;
                        appTraceLog.EventName = ev.event_name;
                        appTraceLog.EventPageId = ev.page_id;
                        appTraceLog.EventRefererPageId = ev.referer_page_id;
                        appTraceLog.EventActionTime = ev.action_time;

                        appTraceLog.CreateTime = DateTime.Now;
                        appTraceLog.Creator = IdWorker.Build(IdType.EmptyGuid);

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }

                    foreach (var ev in rop.exceptionInfos)
                    {
                        var appTraceLog = new AppTraceLog();
                        appTraceLog.Id = IdWorker.Build(IdType.NewGuid);
                        appTraceLog.AppTraceType = E_AppTraceType.Exception;
                        appTraceLog.AppId = rop.device.appinfo.appId;
                        appTraceLog.AppVersion = rop.device.appinfo.appVersion;
                        appTraceLog.AppChannel = rop.device.appinfo.appChannel;
                        appTraceLog.DeviceDensity = rop.device.deviceinfo.deviceDensity;
                        appTraceLog.DeviceId = rop.device.deviceinfo.deviceId;
                        appTraceLog.DeviceLocale = rop.device.deviceinfo.deviceLocale;
                        appTraceLog.DeviceMacAddr = rop.device.deviceinfo.deviceMacAddr;
                        appTraceLog.DeviceModel = rop.device.deviceinfo.deviceModel;
                        appTraceLog.DeviceOsVersion = rop.device.deviceinfo.deviceOsVersion;
                        appTraceLog.DevicePlatform = rop.device.deviceinfo.devicePlatform;
                        appTraceLog.DeviceScreen = rop.device.deviceinfo.deviceScreen;
                        appTraceLog.IpAddr = rop.device.networkinfo.ipAddr;
                        appTraceLog.Wifi = rop.device.networkinfo.wifi;
                        appTraceLog.ExceptionString = ev.exceptionString;
                        appTraceLog.ExceptionSystemModel = ev.systemModel;
                        appTraceLog.ExceptionSystemVersion = ev.systemVersion;
                        appTraceLog.ExceptionPhoneModel = ev.phoneModel;

                        appTraceLog.CreateTime = DateTime.Now;
                        appTraceLog.Creator = IdWorker.Build(IdType.EmptyGuid);

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }
                }

                return true;
            });

            return task;
        }

        public CustomJsonResult EventNotify(string operater, RopMachineEventNotify rop)
        {
            BizFactory.Machine.EventNotify(operater, rop.AppId, rop.MachineId, rop.EventCode, rop.EventRemark, rop.Content);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }


        public CustomJsonResult GetRunExHandleItems(string operater, RupMachineGetRunExHandleItems rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetMachineGetRunExHandleItems();

            ret.ExReasons.Add(new RetMachineGetRunExHandleItems.ExReason { Id = "1", Title = "App异常退出" });
            ret.ExReasons.Add(new RetMachineGetRunExHandleItems.ExReason { Id = "2", Title = "机器出现故障" });
            ret.ExReasons.Add(new RetMachineGetRunExHandleItems.ExReason { Id = "3", Title = "未知原因" });

            var orderSubs = CurrentDb.OrderSub.Where(m => m.SellChannelRefId == rup.MachineId && m.ExIsHappen == true && m.ExIsHandle == false).ToList();

            foreach (var orderSub in orderSubs)
            {
                var exItem = new RetMachineGetRunExHandleItems.ExItem();

                exItem.Id = orderSub.Id;

                var orderSubChilds = CurrentDb.OrderSubChild.Where(m => m.OrderSubId == orderSub.Id).ToList();

                foreach (var orderSubChild in orderSubChilds)
                {
                    var exUnique = new RetMachineGetRunExHandleItems.ExUnique();
                    exUnique.Id = orderSubChild.Id;
                    exUnique.ProductSkuId = orderSubChild.PrdProductSkuId;
                    exUnique.SlotId = orderSubChild.SlotId;
                    exUnique.Quantity = orderSubChild.Quantity;
                    exUnique.Name = orderSubChild.PrdProductSkuName;
                    exUnique.MainImgUrl = orderSubChild.PrdProductSkuMainImgUrl;
                    exUnique.Status = BizFactory.Order.GetPickupStatus(orderSubChild.PickupStatus);
                    exUnique.SignStatus = 0;
                    if (orderSubChild.PickupStatus == E_OrderPickupStatus.Taked || orderSubChild.PickupStatus == E_OrderPickupStatus.ExPickupSignUnTaked && orderSubChild.PickupStatus == E_OrderPickupStatus.ExPickupSignTaked)
                    {
                        exUnique.CanHandle = false;
                    }
                    else
                    {
                        exUnique.CanHandle = true;
                    }

                    exItem.Uniques.Add(exUnique);
                }

                ret.ExItems.Add(exItem);
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }


        public CustomJsonResult HandleRunExItems(string operater, RopMachineHandleRunExItems rop)
        {
            var result = new CustomJsonResult();

            var bizRop = new RopOrderHandleExOrderByMachineSelfTake();
            bizRop.IsRunning = true;
            bizRop.Remark = string.Join(",", rop.Reasons.Select(m => m.Title).ToArray());
            bizRop.Items = rop.Items;

            var bizResult = BizFactory.Order.HandleExOrderByMachineSelfTake(operater, bizRop);
            if (bizResult.Result == ResultType.Success)
            {
                var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();
                MqFactory.Global.PushEventNotify(operater, AppId.STORETERM, machine.CurUseMerchId, machine.CurUseStoreId, rop.MachineId, EventCode.MachineHandleRunEx, "处理运行异常信息，原因：" + bizRop.Remark);
            }



            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "处理成功");
        }
    }
}
