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
    public class MachineService : BaseService
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



            var d_Merch = CurrentDb.Merch.Where(m => m.Id == machine.CurUseMerchId).FirstOrDefault();

            if (d_Merch != null)
            {
                model.MerchId = d_Merch.Id;
                model.MerchName = d_Merch.Name;
                model.CsrQrCode = d_Merch.CsrQrCode;
                model.CsrPhoneNumber = d_Merch.CsrPhoneNumber;
                model.CsrHelpTip = d_Merch.CsrHelpTip;

                model.PayOptions = d_Merch.TermAppPayOptions.ToJsonObject<List<PayOption>>();

                var d_MerchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == machine.CurUseMerchId && m.MachineId == id).FirstOrDefault();
                if (d_MerchMachine != null)
                {
                    model.Name = d_MerchMachine.Name;
                    model.LogoImgUrl = d_MerchMachine.LogoImgUrl;
                }

                var d_Store = CurrentDb.Store.Where(m => m.Id == machine.CurUseStoreId).FirstOrDefault();
                if (d_Store != null)
                {
                    model.StoreId = d_Store.Id;
                    model.StoreName = d_Store.Name;
                }

                var d_Shop = CurrentDb.Shop.Where(m => m.Id == machine.CurUseShopId).FirstOrDefault();
                if (d_Shop != null)
                {
                    model.ShopId = d_Shop.Id;
                    model.ShopName = d_Shop.Name;
                    model.ShopAddress = d_Shop.Address;
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
            var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == merchId && m.MachineId == machineId).FirstOrDefault();

            if (merchMachine == null)
                return true;

            return merchMachine.IsStopUse;
        }

        public void SendStock(string operater, string appId, string merchId, string[] productSkuIds)
        {
            //foreach(var machineId in machineIds)
            // {
            //     SendStock(operater, appId, merchId,machineId);
            // }
        }

        public void SendHomeBanners(string operater, string appId, string merchId, string[] machineId)
        {
            //var banners = BizFactory.Machine.GetHomeBanners(machineId);
            //PushService.SendHomeBanners(operater, appId, merchId, machineId, banners);
        }

        public void SendHomeLogo(string operater, string appId, string merchId, string machineId, string logoImgUrl)
        {
            var content = new { url = logoImgUrl };
            PushService.SendHomeLogo(operater, appId, merchId, machineId, content);
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

            return PushService.QueryStatus(operater, appId, merchId, machineId, messageId);
        }


        //public CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string machineId, string orderId)
        //{
        //    var orderDetails = BizFactory.Order.GetOrderProductSkuByPickup(orderId, machineId);
        //    var content = new { orderId = orderId, status = E_OrderStatus.Payed, orderDetails =  orderDetails };
        //    return PushService.SendPaySuccess(operater, appId, merchId, machineId, content);
        //}

        public CustomJsonResult EventNotify(string operater, string appId, string machineId, string eventCode, string eventRemark, object content)
        {
            MqFactory.Global.PushEventNotify(operater, appId, machineId, eventCode, eventRemark, content);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
