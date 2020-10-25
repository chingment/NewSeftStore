using LocalS.BLL.Mq;
using LocalS.BLL.Push;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace LocalS.BLL.Biz
{
    public class MachineService : BaseDbContext
    {
        public MachineInfoModel GetOne(string id)
        {
            var model = new MachineInfoModel();

            var machine = CurrentDb.Machine.Where(m => m.Id == id).FirstOrDefault();

            if (machine == null)
                return null;

            model.MachineId = machine.Id;
            model.DeviceId = machine.DeviceId;
            model.Name = machine.Name;
            model.MainImgUrl = machine.MainImgUrl;
            model.LogoImgUrl = machine.LogoImgUrl;
            model.JPushRegId = machine.JPushRegId;
            model.RunStatus = machine.RunStatus;
            model.LastRequestTime = machine.LastRequestTime;
            model.AppVersion = machine.AppVersionName;
            model.CtrlSdkVersion = machine.CtrlSdkVersionCode;
            model.KindIsHidden = machine.KindIsHidden;
            model.KindRowCellSize = machine.KindRowCellSize;
            model.IsTestMode = machine.IsTestMode;
            model.CameraByChkIsUse = machine.CameraByChkIsUse;
            model.CameraByJgIsUse = machine.CameraByJgIsUse;
            model.CameraByRlIsUse = machine.CameraByRlIsUse;
            model.ExIsHas = machine.ExIsHas;
            model.OstVern = machine.OstVern;
            model.MstVern = machine.MstVern;

            model.ImIsUse = machine.ImIsUse;
            model.ImPartner = machine.ImPartner;
            model.ImUserName = machine.ImUserName;
            model.ImPassword = machine.ImPassword;

            var machineCabinets = CurrentDb.MachineCabinet.Where(m => m.MachineId == id && m.IsUse == true).OrderByDescending(m => m.Priority).ToList();

            foreach (var machineCabinet in machineCabinets)
            {
                var cabinet = new CabinetInfoModel();
                cabinet.CabinetId = machineCabinet.CabinetId;
                cabinet.Name = machineCabinet.CabinetName;
                cabinet.RowColLayout = machineCabinet.RowColLayout;
                cabinet.Priority = machineCabinet.Priority;
                cabinet.ComId = machineCabinet.ComId;
                model.Cabinets.Add(cabinet.CabinetId, cabinet);
            }


            model.FingerVeinner.FingerVeinnerId = "FV";
            model.FingerVeinner.IsUse = machine.FingerVeinnerIsUse;

            model.Scanner.ScannerId = "SC";
            model.Scanner.IsUse = machine.SannerIsUse;
            model.Scanner.ComId = machine.SannerComId;



            var merch = CurrentDb.Merch.Where(m => m.Id == machine.CurUseMerchId).FirstOrDefault();

            if (merch != null)
            {
                model.MerchId = merch.Id;
                model.MerchName = merch.Name;
                model.CsrQrCode = merch.CsrQrCode;
                model.CsrPhoneNumber = merch.CsrPhoneNumber;
                model.CsrHelpTip = merch.CsrHelpTip;

                model.PayOptions = merch.TermAppPayOptions.ToJsonObject<List<PayOption>>();

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == machine.CurUseMerchId && m.MachineId == id).FirstOrDefault();
                if (merchMachine != null)
                {
                    model.Name = merchMachine.Name;
                    model.LogoImgUrl = merchMachine.LogoImgUrl;
                }

                var merchStore = BizFactory.Store.GetOne(machine.CurUseStoreId);
                if (merchStore != null)
                {
                    model.StoreId = merchStore.StoreId;
                    model.StoreName = merchStore.Name;
                    model.StoreAddress = merchStore.Address;
                }
            }

            return model;
        }

        public List<BannerModel> GetHomeBanners(string id)
        {
            var bannerModels = new List<BannerModel>();

            var machine = BizFactory.Machine.GetOne(id);

            var adContentIds = CurrentDb.AdContentBelong.Where(m => m.MerchId == machine.MerchId && m.AdSpaceId == E_AdSpaceId.MachineHomeBanner && m.BelongType == E_AdSpaceBelongType.Machine && m.BelongId == id).Select(m => m.AdContentId).ToArray();

            if (adContentIds != null && adContentIds.Length > 0)
            {
                var adContents = CurrentDb.AdContent.Where(m => adContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();


                foreach (var item in adContents)
                {
                    bannerModels.Add(new BannerModel { Url = item.Url });
                }
            }

            return bannerModels;
        }


        public Dictionary<string, AdModel> GetAds(string id)
        {
            var ads = new Dictionary<string, AdModel>();

            var machine = BizFactory.Machine.GetOne(id);


            var adSpaces = CurrentDb.AdSpace.Where(m => m.Id == E_AdSpaceId.MachineHomeBanner).ToList();

            foreach (var adSpace in adSpaces)
            {
                var ad = new AdModel();
                ad.AdId = adSpace.Id;
                ad.Name = adSpace.Name;
                var adContentIds = CurrentDb.AdContentBelong.Where(m => m.MerchId == machine.MerchId && m.AdSpaceId == adSpace.Id && m.BelongType == E_AdSpaceBelongType.Machine && m.BelongId == id).Select(m => m.AdContentId).ToArray();

                if (adContentIds != null && adContentIds.Length > 0)
                {
                    var adContents = CurrentDb.AdContent.Where(m => adContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();

                    foreach (var item in adContents)
                    {
                        ad.Contents.Add(new AdContentModel { DataType = "image", DataUrl = item.Url });
                    }
                }

                ads.Add(((int)adSpace.Id).ToString(), ad);
            }



            return ads;
        }

        public bool IsStopUse(string merchId, string machineId)
        {
            bool isFlag = true;

            var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == machineId).FirstOrDefault();

            if (merchMachine == null)
                return true;

            return merchMachine.IsStopUse;
        }

        public void SendUpdateProductSkuStock(string operater, string appId, string merchId, string machineId, UpdateMachineProdcutSkuStockModel updateProdcutSkuStock)
        {
            PushService.SendUpdateProductSkuStock(operater, appId, merchId, machineId, updateProdcutSkuStock);
        }

        public void SendUpdateHomeBanners(string operater, string appId, string merchId, string machineId)
        {
            var banners = BizFactory.Machine.GetHomeBanners(machineId);
            PushService.SendUpdateMachineHomeBanners(operater, appId, merchId, machineId, banners);
        }

        public void SendUpdateHomeLogo(string operater, string appId, string merchId, string machineId, string logoImgUrl)
        {
            var content = new { url = logoImgUrl };
            PushService.SendUpdateMachineHomeLogo(operater, appId, merchId, machineId, content);
        }

        public CustomJsonResult SendSysReboot(string operater, string appId, string merchId, string machineId)
        {
            if (IsStopUse(merchId, machineId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器已停止使用");
            }

            return PushService.SendSysReboot(operater, appId, merchId, machineId);
        }

        public CustomJsonResult SendSysShutdown(string operater, string appId, string merchId, string machineId)
        {
            if (IsStopUse(merchId, machineId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器已停止使用");
            }

            return PushService.SendSysShutdown(operater, appId, merchId, machineId);
        }

        public CustomJsonResult SendSysSetStatus(string operater, string appId, string merchId, string machineId, int status, string helpTip)
        {
            if (IsStopUse(merchId, machineId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器已停止使用");
            }

            var content = new { status = status, helpTip = helpTip };
            return PushService.SendSysSetStatus(operater, appId, merchId, machineId, content);
        }

        public CustomJsonResult SendDsx01OpenPickupDoor(string operater, string appId, string merchId, string machineId)
        {
            if (IsStopUse(merchId, machineId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器已停止使用");
            }

            return PushService.SendDsx01OpenPickupDoor(operater, appId, merchId, machineId);
        }

        public CustomJsonResult QueryMsgPushResult(string operater, string appId, string merchId, string machineId, string messageId)
        {
            if (IsStopUse(merchId, machineId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该机器已停止使用");
            }

            return PushService.QueryMsgPushResult(operater, appId, merchId, machineId, messageId);
        }


        //public CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string machineId, string orderId)
        //{
        //    var orderDetails = BizFactory.Order.GetOrderProductSkuByPickup(orderId, machineId);
        //    var content = new { orderId = orderId, status = E_OrderStatus.Payed, orderDetails =  orderDetails };
        //    return PushService.SendPaySuccess(operater, appId, merchId, machineId, content);
        //}

        public CustomJsonResult EventNotify(string operater, string appId, string machineId, string eventCode, string eventRemark, object content)
        {
            var machine = BizFactory.Machine.GetOne(machineId);
            MqFactory.Global.PushEventNotify(operater, appId, machine.MerchId, machine.StoreId, machineId, eventCode, eventRemark, content);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
