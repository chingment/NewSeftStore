using LocalS.BLL;
using LocalS.BLL.Biz;
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


            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine == null)
            {
                machine = new Machine();
                machine.Id = rop.MachineId;
                machine.JPushRegId = rop.JPushRegId;
                machine.MacAddress = rop.MacAddress;
                machine.AppVersionCode = rop.AppVersionCode;
                machine.AppVersionName = rop.AppVersionName;
                machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                machine.CreateTime = DateTime.Now;
                machine.Creator = GuidUtil.Empty();
                CurrentDb.Machine.Add(machine);
                CurrentDb.SaveChanges();
            }
            else
            {
                machine.JPushRegId = rop.JPushRegId;
                machine.MacAddress = rop.MacAddress;
                machine.AppVersionCode = rop.AppVersionCode;
                machine.AppVersionName = rop.AppVersionName;
                machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                machine.MendTime = DateTime.Now;
                machine.Mender = GuidUtil.Empty();
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

            var machineInfo = BizFactory.Machine.GetOne(rop.MachineId);
            ret.Machine.Id = machineInfo.Id;
            ret.Machine.Name = machineInfo.Name;
            ret.Machine.LogoImgUrl = machineInfo.LogoImgUrl;
            ret.Machine.MerchName = machineInfo.MerchName;
            ret.Machine.StoreName = machineInfo.StoreName;
            ret.Machine.CsrQrCode = machineInfo.CsrQrCode;
            ret.Machine.CabinetId_1 = machineInfo.CabinetId_1;
            ret.Machine.CabinetName_1 = machineInfo.CabinetName_1;
            ret.Machine.CabinetRowColLayout_1 = machineInfo.CabinetRowColLayout_1;

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

            var prdKinds = CurrentDb.PrdKind.Where(m => m.MerchId == merchId && m.Depth == 1 && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            foreach (var prdKind in prdKinds)
            {
                var prdKindModel = new ProductKindModel();
                prdKindModel.Id = prdKind.Id;
                prdKindModel.Name = prdKind.Name;

                var productIds = CurrentDb.PrdProductKind.Where(m => m.PrdKindId == prdKind.Id).Select(m => m.PrdProductId).Distinct().ToList();
                if (productIds.Count > 0)
                {
                    var productSkuIds = CurrentDb.SellChannelStock.Where(m => productIds.Contains(m.PrdProductId)).Select(m => m.PrdProductSkuId).Distinct().ToList();
                    if (productSkuIds.Count > 0)
                    {
                        prdKindModel.Childs = productSkuIds;
                        productKindModels.Add(prdKindModel);
                    }
                }

            }

            return productKindModels;
        }

        public CustomJsonResult UpdateInfo(RopMachineUpdateInfo rop)
        {
            var result = new CustomJsonResult();

            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "更新失败，找不到机器信息");
            }

            switch (rop.DataType)
            {
                case 1:
                    break;
            }

            //machine.Lat = rop.Lat;
            //machine.Lng = rop.Lng;




            CurrentDb.SaveChanges();

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "更新成功");
        }

        public CustomJsonResult Login(RopMachineLogin rop)
        {

            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败，该机器未登记");
            }

            var sysMerchantUser = CurrentDb.SysMerchUser.Where(m => m.UserName == rop.UserName).FirstOrDefault();

            if (sysMerchantUser == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败,用户名不存在");
            }

            var isPasswordCorrect = PassWordHelper.VerifyHashedPassword(sysMerchantUser.PasswordHash, rop.Password);

            if (!isPasswordCorrect)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "登录失败,用户密码错误");
            }

            if (sysMerchantUser.MerchId != machine.CurUseMerchId)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "帐号与商户不对应");
            }

            var ret = new RetMachineLogin();

            ret.Token = GuidUtil.New();
            ret.UserName = sysMerchantUser.UserName;
            ret.FullName = sysMerchantUser.FullName;

            var tokenInfo = new TokenInfo();
            tokenInfo.UserId = sysMerchantUser.Id;
            tokenInfo.MerchId = sysMerchantUser.MerchId;

            SSOUtil.SetTokenInfo(ret.Token, tokenInfo, new TimeSpan(3, 0, 0));

            LogAction(sysMerchantUser.Id, rop.MachineId, "login", "登录机器管理后台");

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "登录成功", ret);

        }

        public CustomJsonResult Logout(string operater, string token, RopMachineLogout rop)
        {

            SSOUtil.Quit(token);

            LogAction(operater, rop.MachineId, "logout", "退出机器管理后台");

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "退出成功");

        }

        public CustomJsonResult SendRunStatus(RopMachineSendRunStatus rop)
        {
            CustomJsonResult result = new CustomJsonResult();


            var machine = CurrentDb.Machine.Where(m => m.Id == rop.MachineId).FirstOrDefault();

            if (machine != null)
            {
                switch (rop.Status)
                {
                    case "running":
                        machine.RunStatus = E_MachineRunStatus.Running;
                        machine.LastRequestTime = DateTime.Now;
                        break;
                    case "setting":
                        machine.RunStatus = E_MachineRunStatus.Setting;
                        machine.LastRequestTime = DateTime.Now;
                        break;
                }

                CurrentDb.SaveChanges();
            }

            return result;
        }

        public Task<bool> LogAction(string operater, string machineId, string action, string remark)
        {
            var task = Task.Run(() =>
            {

                var machine = BizFactory.Machine.GetOne(machineId);

                var machineOperateLog = new MachineOperateLog();
                machineOperateLog.Id = GuidUtil.New();

                if (machine != null)
                {
                    machineOperateLog.MerchId = machine.MerchId;
                    machineOperateLog.StoreId = machine.StoreId;
                }

                machineOperateLog.MachineId = machineId;
                machineOperateLog.OperaterUserId = operater;
                machineOperateLog.Action = action;
                machineOperateLog.Remark = remark;
                machineOperateLog.Creator = operater;
                machineOperateLog.CreateTime = DateTime.Now;


                CurrentDb.MachineOperateLog.Add(machineOperateLog);
                CurrentDb.SaveChanges();

                return true;
            });

            return task;
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
                        appTraceLog.Id = GuidUtil.New();
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
                        appTraceLog.Creator = GuidUtil.Empty();

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }

                    foreach (var pa in rop.pages)
                    {
                        var appTraceLog = new AppTraceLog();
                        appTraceLog.Id = GuidUtil.New();
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
                        appTraceLog.Creator = GuidUtil.Empty();

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }

                    foreach (var ev in rop.events)
                    {
                        var appTraceLog = new AppTraceLog();
                        appTraceLog.Id = GuidUtil.New();
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
                        appTraceLog.Creator = GuidUtil.Empty();

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }

                    foreach (var ev in rop.exceptionInfos)
                    {
                        var appTraceLog = new AppTraceLog();
                        appTraceLog.Id = GuidUtil.New();
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
                        appTraceLog.Creator = GuidUtil.Empty();

                        CurrentDb.AppTraceLog.Add(appTraceLog);
                        CurrentDb.SaveChanges();
                    }
                }

                return true;
            });

            return task;
        }
    }
}
