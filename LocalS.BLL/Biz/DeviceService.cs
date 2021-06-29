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
    public class DeviceService : BaseService
    {
        public DeviceModel GetOne(string id)
        {
            var model = new DeviceModel();

            var d_Device = CurrentDb.Device.Where(m => m.Id == id).FirstOrDefault();

            if (d_Device == null)
                return null;

            model.DeviceId = d_Device.Id;
            model.Name = d_Device.Name;
            model.MainImgUrl = d_Device.MainImgUrl;
            model.LogoImgUrl = d_Device.LogoImgUrl;
            model.RunStatus = d_Device.RunStatus;
            model.LastRequestTime = d_Device.LastRequestTime;
            model.AppVersion = d_Device.AppVersionName;
            model.CtrlSdkVersion = d_Device.CtrlSdkVersionCode;
            model.KindIsHidden = d_Device.KindIsHidden;
            model.KindRowCellSize = d_Device.KindRowCellSize;
            model.IsTestMode = d_Device.IsTestMode;
            model.CameraByChkIsUse = d_Device.CameraByChkIsUse;
            model.CameraByJgIsUse = d_Device.CameraByJgIsUse;
            model.CameraByRlIsUse = d_Device.CameraByRlIsUse;
            model.ExIsHas = d_Device.ExIsHas;
            model.OstVern = d_Device.OstVern;
            model.MstVern = d_Device.MstVern;
            model.ImIsUse = d_Device.ImIsUse;
            model.ImPartner = d_Device.ImPartner;
            model.ImUserName = d_Device.ImUserName;
            model.ImPassword = d_Device.ImPassword;

            var d_Cabinets = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == id && m.IsUse == true).OrderByDescending(m => m.Priority).ToList();

            foreach (var d_Cabinet in d_Cabinets)
            {
                var cabinet = new CabinetModel();
                cabinet.CabinetId = d_Cabinet.CabinetId;
                cabinet.Name = d_Cabinet.CabinetName;
                cabinet.RowColLayout = d_Cabinet.RowColLayout;
                cabinet.Priority = d_Cabinet.Priority;
                cabinet.ComId = d_Cabinet.ComId;
                model.Cabinets.Add(cabinet.CabinetId, cabinet);
            }


            model.FingerVeinner.FingerVeinnerId = "FV";
            model.FingerVeinner.IsUse = d_Device.FingerVeinnerIsUse;

            model.Scanner.ScannerId = "SC";
            model.Scanner.IsUse = d_Device.SannerIsUse;
            model.Scanner.ComId = d_Device.SannerComId;



            var d_Merch = CurrentDb.Merch.Where(m => m.Id == d_Device.CurUseMerchId).FirstOrDefault();

            if (d_Merch != null)
            {
                model.MerchId = d_Merch.Id;
                model.MerchName = d_Merch.Name;
                model.CsrQrCode = d_Merch.CsrQrCode;
                model.CsrPhoneNumber = d_Merch.CsrPhoneNumber;
                model.CsrHelpTip = d_Merch.CsrHelpTip;

                model.PayOptions = d_Merch.TermAppPayOptions.ToJsonObject<List<PayOption>>();

                var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == d_Device.CurUseMerchId && m.DeviceId == id).FirstOrDefault();
                if (d_MerchDevice != null)
                {
                    model.Name = d_MerchDevice.Name;
                    model.LogoImgUrl = d_MerchDevice.LogoImgUrl;
                }

                var d_Store = CurrentDb.Store.Where(m => m.Id == d_Device.CurUseStoreId).FirstOrDefault();
                if (d_Store != null)
                {
                    model.StoreId = d_Store.Id;
                    model.StoreName = d_Store.Name;
                }

                var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_Device.CurUseShopId).FirstOrDefault();
                if (d_Shop != null)
                {
                    model.ShopId = d_Shop.Id;
                    model.ShopName = d_Shop.Name;
                    model.ShopAddress = d_Shop.Address;
                }
            }

            return model;
        }

        public Dictionary<string, AdModel> GetAds(string id)
        {
            var ads = new Dictionary<string, AdModel>();

            var d_Device = CurrentDb.Device.Where(m => m.Id == id).FirstOrDefault();

            if (d_Device == null)
                return ads;

            if (string.IsNullOrEmpty(d_Device.CurUseMerchId))
                return ads;

            var adSpaces = CurrentDb.AdSpace.Where(m => m.Id == E_AdSpaceId.DeviceHomeBanner).ToList();

            foreach (var adSpace in adSpaces)
            {
                var ad = new AdModel();
                ad.AdId = adSpace.Id;
                ad.Name = adSpace.Name;
                var adContentIds = CurrentDb.AdContentBelong.Where(m => m.Status == E_AdContentBelongStatus.Normal && m.MerchId == d_Device.CurUseMerchId && m.AdSpaceId == adSpace.Id && m.BelongType == E_AdSpaceBelongType.Device && m.BelongId == id).Select(m => m.AdContentId).ToArray();

                if (adContentIds != null && adContentIds.Length > 0)
                {
                    var adContents = CurrentDb.AdContent.Where(m => adContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();

                    foreach (var item in adContents)
                    {
                        ad.Contents.Add(new AdModel.ContentModel { DataType = "image", DataUrl = item.Url });
                    }
                }

                ads.Add(((int)adSpace.Id).ToString(), ad);
            }



            return ads;
        }

        public bool IsStopUse(string merchId, string deviceId)
        {
            var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == merchId && m.DeviceId == deviceId).FirstOrDefault();

            if (d_MerchDevice == null)
            {
                LogUtil.Warn("找不到记录");
                return true;
            }

            return d_MerchDevice.IsStopUse;
        }

        public void SendStock(string operater, string appId, string merchId, string deviceId, List<DeviceSkuStockModel> contents)
        {
            PushService.SendStock(operater, appId, merchId, deviceId, contents);
        }

        public void SendStock(string operater, string appId, string merchId, string deviceId, DeviceSkuStockModel content)
        {
            List<DeviceSkuStockModel> contents = new List<DeviceSkuStockModel>();
            contents.Add(content);
            SendStock(operater, appId, merchId, deviceId, contents);
        }

        public void SendStock(string operater, string appId, string merchId, string deviceId)
        {
            //PushService.SendStock(operater, appId, merchId, deviceId);
        }

        public void SendAds(string operater, string appId, string merchId, string[] deviceIds)
        {
            var task = System.Threading.Tasks.Task.Run(() =>
            {
                foreach (var deviceId in deviceIds)
                {
                    var ads = BizFactory.Device.GetAds(deviceId);
                    PushService.SendAds(operater, appId, merchId, deviceId, ads);
                }
            });
        }

        public void SendHomeLogo(string operater, string appId, string merchId, string deviceId, string logoImgUrl)
        {
            var content = new { url = logoImgUrl };
            PushService.SendHomeLogo(operater, appId, merchId, deviceId, content);
        }

        public CustomJsonResult SendSysReboot(string operater, string appId, string merchId, string deviceId)
        {
            if (IsStopUse(merchId, deviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            return PushService.SendSysReboot(operater, appId, merchId, deviceId);
        }

        public CustomJsonResult SendSysShutdown(string operater, string appId, string merchId, string deviceId)
        {
            if (IsStopUse(merchId, deviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            return PushService.SendSysShutdown(operater, appId, merchId, deviceId);
        }

        public CustomJsonResult SendSysSetStatus(string operater, string appId, string merchId, string deviceId, int status, string helpTip)
        {
            if (IsStopUse(merchId, deviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            var content = new { status = status, helpTip = helpTip };
            return PushService.SendSysSetStatus(operater, appId, merchId, deviceId, content);
        }

        public CustomJsonResult SendDsx01OpenPickupDoor(string operater, string appId, string merchId, string deviceId)
        {
            if (IsStopUse(merchId, deviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            return PushService.SendDsx01OpenPickupDoor(operater, appId, merchId, deviceId);
        }

        public CustomJsonResult QueryMsgPushResult(string operater, string appId, string merchId, string deviceId, string messageId)
        {
            if (IsStopUse(merchId, deviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            return PushService.QueryStatus(operater, appId, merchId, deviceId, messageId);
        }


        //public CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string deviceId, string orderId)
        //{
        //    var orderDetails = BizFactory.Order.GetOrderSkuByPickup(orderId, deviceId);
        //    var content = new { orderId = orderId, status = E_OrderStatus.Payed, orderDetails =  orderDetails };
        //    return PushService.SendPaySuccess(operater, appId, merchId, deviceId, content);
        //}

        public CustomJsonResult EventNotify(string operater, string appId, string deviceId, string eventCode, string eventRemark, object content)
        {
            MqFactory.Global.PushEventNotify(operater, appId, deviceId, eventCode, eventRemark, content);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
