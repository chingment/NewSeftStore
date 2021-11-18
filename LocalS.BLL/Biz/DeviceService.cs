using LocalS.BLL.Mq;
using LocalS.BLL.Push;
using LocalS.BLL.Task;
using LocalS.Entity;
using Lumos;
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
            var m_Device = new DeviceModel();

            var d_Device = CurrentDb.Device.Where(m => m.Id == id).FirstOrDefault();

            if (d_Device == null)
                return null;

            m_Device.DeviceId = d_Device.Id;
            m_Device.Name = d_Device.Name;
            m_Device.Type = d_Device.Type;
            m_Device.MainImgUrl = d_Device.MainImgUrl;
            m_Device.LogoImgUrl = d_Device.LogoImgUrl;
            m_Device.RunStatus = d_Device.RunStatus;
            m_Device.LastRequestTime = d_Device.LastRequestTime;
            m_Device.AppVersion = d_Device.AppVersionName;
            m_Device.CtrlSdkVersion = d_Device.CtrlSdkVersionCode;
            m_Device.KindIsHidden = d_Device.KindIsHidden;
            m_Device.KindRowCellSize = d_Device.KindRowCellSize;
            m_Device.IsTestMode = d_Device.IsTestMode;
            m_Device.CameraByChkIsUse = d_Device.CameraByChkIsUse;
            m_Device.CameraByJgIsUse = d_Device.CameraByJgIsUse;
            m_Device.CameraByRlIsUse = d_Device.CameraByRlIsUse;
            m_Device.ExIsHas = d_Device.ExIsHas;
            m_Device.OstVern = d_Device.OstVern;
            m_Device.MstVern = d_Device.MstVern;
            m_Device.ImIsUse = d_Device.ImIsUse;
            m_Device.ImPartner = d_Device.ImPartner;
            m_Device.ImUserName = d_Device.ImUserName;
            m_Device.ImPassword = d_Device.ImPassword;

            var d_Cabinets = CurrentDb.DeviceCabinet.Where(m => m.DeviceId == id && m.IsUse == true).OrderByDescending(m => m.Priority).ToList();

            foreach (var d_Cabinet in d_Cabinets)
            {
                var m_Cabinet = new CabinetModel();
                m_Cabinet.CabinetId = d_Cabinet.CabinetId;
                m_Cabinet.Name = d_Cabinet.CabinetName;
                m_Cabinet.RowColLayout = d_Cabinet.RowColLayout;
                m_Cabinet.ComId = d_Cabinet.ComId;
                m_Cabinet.ComBaud = d_Cabinet.ComBaud;
                m_Cabinet.Priority = d_Cabinet.Priority;
                m_Device.Cabinets.Add(m_Cabinet.CabinetId, m_Cabinet);
            }


            m_Device.FingerVeinner.FingerVeinnerId = "FV";
            m_Device.FingerVeinner.IsUse = d_Device.FingerVeinnerIsUse;

            m_Device.Scanner.ScannerId = "SC";
            m_Device.Scanner.IsUse = d_Device.SannerIsUse;
            m_Device.Scanner.ComId = d_Device.SannerComId;

            m_Device.Lights.Add("cb", d_Device.CbLight);

            var d_Merch = CurrentDb.Merch.Where(m => m.Id == d_Device.CurUseMerchId).FirstOrDefault();

            if (d_Merch != null)
            {
                m_Device.MerchId = d_Merch.Id;
                m_Device.MerchName = d_Merch.Name;
                m_Device.CsrQrCode = d_Merch.CsrQrCode;
                m_Device.CsrPhoneNumber = d_Merch.CsrPhoneNumber;
                m_Device.CsrHelpTip = d_Merch.CsrHelpTip;

                var d_MerchDevice = CurrentDb.MerchDevice.Where(m => m.MerchId == d_Device.CurUseMerchId && m.DeviceId == id).FirstOrDefault();
                if (d_MerchDevice != null)
                {
                    m_Device.Name = d_Device.Name;
                    m_Device.LogoImgUrl = d_MerchDevice.LogoImgUrl;
                }

                var d_Store = CurrentDb.Store.Where(m => m.Id == d_Device.CurUseStoreId).FirstOrDefault();
                if (d_Store != null)
                {
                    m_Device.StoreId = d_Store.Id;
                    m_Device.StoreName = d_Store.Name;
                    m_Device.PayOptions = d_Store.TermAppPayOptions.ToJsonObject<List<PayOption>>();
                }

                var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_Device.CurUseShopId).FirstOrDefault();
                if (d_Shop != null)
                {
                    m_Device.ShopId = d_Shop.Id;
                    m_Device.ShopName = d_Shop.Name;
                    m_Device.ShopAddress = d_Shop.Address;
                }
            }

            return m_Device;
        }

        //public DeviceBelongModel GetBelong(string id)
        //{
        //    var m_Device = new DeviceBelongModel();

        //    var d_Device = CurrentDb.Device.Where(m => m.Id == id).FirstOrDefault();

        //    if (d_Device == null)
        //        return null;

        //    m_Device.DeviceId = d_Device.Id;

        //    var d_Merch = CurrentDb.Merch.Where(m => m.Id == d_Device.CurUseMerchId).FirstOrDefault();

        //    if (d_Merch != null)
        //    {
        //        m_Device.MerchId = d_Merch.Id;
        //        m_Device.MerchName = d_Merch.Name;

        //        var d_Store = CurrentDb.Store.Where(m => m.Id == d_Device.CurUseStoreId).FirstOrDefault();
        //        if (d_Store != null)
        //        {
        //            m_Device.StoreId = d_Store.Id;
        //            m_Device.StoreName = d_Store.Name;
        //        }

        //        var d_Shop = CurrentDb.Shop.Where(m => m.Id == d_Device.CurUseShopId).FirstOrDefault();
        //        if (d_Shop != null)
        //        {
        //            m_Device.ShopId = d_Shop.Id;
        //            m_Device.ShopName = d_Shop.Name;
        //        }
        //    }

        //    return m_Device;
        //}


        public Dictionary<string, AdModel> GetAds(string id)
        {
            var m_Ads = new Dictionary<string, AdModel>();

            var d_Device = CurrentDb.Device.Where(m => m.Id == id).FirstOrDefault();

            if (d_Device == null)
                return m_Ads;

            if (string.IsNullOrEmpty(d_Device.CurUseMerchId))
                return m_Ads;

            var d_AdSpaces = CurrentDb.AdSpace.Where(m => m.BelongType == E_AdSpaceBelongType.Device).ToList();

            foreach (var d_AdSpace in d_AdSpaces)
            {
                var m_Ad = new AdModel();
                m_Ad.AdId = d_AdSpace.Id;
                m_Ad.Name = d_AdSpace.Name;

                var l_AdContentIds = CurrentDb.AdContentBelong.Where(m => m.Status == E_AdContentBelongStatus.Normal && m.MerchId == d_Device.CurUseMerchId && m.AdSpaceId == d_AdSpace.Id && m.BelongType == E_AdSpaceBelongType.Device && m.BelongId == id).Select(m => m.AdContentId).ToArray();

                if (l_AdContentIds != null && l_AdContentIds.Length > 0)
                {
                    var d_AdContents = CurrentDb.AdContent.Where(m => l_AdContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();

                    foreach (var d_AdContent in d_AdContents)
                    {
                        m_Ad.Contents.Add(new AdModel.ContentModel { DataType = "image", DataUrl = d_AdContent.Url });
                    }
                }

                m_Ads.Add(((int)d_AdSpace.Id).ToString(), m_Ad);
            }



            return m_Ads;
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

        public void SendUpdateStock(string operater, string appId, string merchId, string deviceId, object pms)
        {
            SendCommand(operater, appId, merchId, deviceId, "update_stock", pms);
        }

        public void SendAds(string operater, string appId, string merchId, string[] deviceIds)
        {
            var task = System.Threading.Tasks.Task.Run(() =>
            {
                foreach (var deviceId in deviceIds)
                {
                    var ads = BizFactory.Device.GetAds(deviceId);
                    SendCommand(operater, appId, merchId, deviceId, "update_ads", ads);
                }
            });
        }

        public CustomJsonResult SendRebootSys(string operater, string appId, string merchId, string deviceId)
        {
            return SendCommand(operater, appId, merchId, deviceId, "reboot_sys");
        }

        public CustomJsonResult SendShutdownSys(string operater, string appId, string merchId, string deviceId)
        {
            return SendCommand(operater, appId, merchId, deviceId, "shutdown_sys");
        }

        public CustomJsonResult SendSetSysStatus(string operater, string appId, string merchId, string deviceId, int status, string helpTip)
        {
            var pms = new { status = status, helpTip = helpTip };
            return SendCommand(operater, appId, merchId, deviceId, "set_sys_status", pms);
        }

        public CustomJsonResult SendSetSysParams(string operater, string appId, string merchId, string deviceId, object pms)
        {
            return SendCommand(operater, appId, merchId, deviceId, "set_sys_params", pms);
        }

        public CustomJsonResult SendOpenPickupDoor(string operater, string appId, string merchId, string deviceId)
        {
            return SendCommand(operater, appId, merchId, deviceId, "open_pickup_door");
        }

        public CustomJsonResult SendUpdateApp(string operater, string appId, string merchId, string deviceId)
        {
            return SendCommand(operater, appId, merchId, deviceId, "update_app");
        }

        public CustomJsonResult SendDeviceShip(string operater, string appId, string merchId, string deviceId, object pms)
        {
            return SendCommand(operater, appId, merchId, deviceId, "device_ship", pms);
        }

        public CustomJsonResult SendCommand(string operater, string appId, string merchId, string deviceId, string method, object pms = null)
        {
            if (IsStopUse(merchId, deviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            return MqttService.GetInstance().Send(operater, appId, merchId, deviceId, method, pms);

        }

        public CustomJsonResult QueryMsgPushResult(string operater, string appId, string merchId, string deviceId, string messageId)
        {
            if (IsStopUse(merchId, deviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "该设备已停止使用");
            }

            return MqttService.GetInstance().QueryStatus(operater, appId, merchId, deviceId, messageId);
        }


        //public CustomJsonResult SendPaySuccess(string operater, string appId, string merchId, string deviceId, string orderId)
        //{
        //    var orderDetails = BizFactory.Order.GetOrderSkuByPickup(orderId, deviceId);
        //    var content = new { orderId = orderId, status = E_OrderStatus.Payed, orderDetails =  orderDetails };
        //    return PushService.SendPaySuccess(operater, appId, merchId, deviceId, content);
        //}

        public CustomJsonResult EventNotify(string operater, string appId, string deviceId, string eventCode, string eventRemark,int eventMsgId, string eventMsgMode, object content)
        {
            MqFactory.Global.PushEventNotify(operater, appId, deviceId, eventCode, eventRemark, eventMsgId, eventMsgMode, content);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
