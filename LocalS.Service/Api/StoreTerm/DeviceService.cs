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
    public class DeviceService : BaseService
    {
        public IResult InitData(RopDeviceInitData rop)
        {
            var result = new CustomJsonResult();

            var ret = new RetDeviceInitData();

            if (string.IsNullOrEmpty(rop.DeviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备号为空");
            }

            var d_Device = CurrentDb.Device.Where(m => m.Id == rop.DeviceId).FirstOrDefault();

            if (d_Device == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备号未登记");
            }

            if (string.IsNullOrEmpty(d_Device.CurUseMerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定商户");
            }

            if (string.IsNullOrEmpty(d_Device.CurUseStoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定店铺");
            }

            if (string.IsNullOrEmpty(d_Device.CurUseShopId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备未绑定门店");
            }

            //d_Device.MacAddress = rop.MacAddress;
            //d_Device.ImeiId = rop.ImeiId;
            d_Device.AppVersionCode = rop.AppVersionCode;
            d_Device.AppVersionName = rop.AppVersionName;
            d_Device.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
            d_Device.MendTime = DateTime.Now;
            d_Device.Mender = IdWorker.Build(IdType.EmptyGuid);
            CurrentDb.SaveChanges();

            var m_Device = BizFactory.Device.GetOne(d_Device.Id);

            ret.Device.DeviceId = m_Device.DeviceId;
            ret.Device.Name = m_Device.Name;
            ret.Device.Type = m_Device.Type;
            ret.Device.LogoImgUrl = m_Device.LogoImgUrl;
            ret.Device.MerchName = m_Device.MerchName;
            ret.Device.StoreName = m_Device.StoreName;
            ret.Device.ShopName = m_Device.ShopName;
            ret.Device.ShopAddress = m_Device.ShopAddress;
            ret.Device.Consult.CsrQrCode = m_Device.CsrQrCode;
            ret.Device.Consult.CsrPhoneNumber = m_Device.CsrPhoneNumber;
            ret.Device.Consult.CsrHelpTip = m_Device.CsrHelpTip;
            ret.Device.Cabinets = m_Device.Cabinets;
            ret.Device.PayOptions = m_Device.PayOptions;
            ret.Device.CameraByChkIsUse = m_Device.CameraByChkIsUse;
            ret.Device.CameraByJgIsUse = m_Device.CameraByJgIsUse;
            ret.Device.CameraByRlIsUse = m_Device.CameraByRlIsUse;
            ret.Device.ExIsHas = m_Device.ExIsHas;
            ret.Device.OstVern = m_Device.OstVern;
            ret.Device.MstVern = m_Device.MstVern;
            ret.Device.Scanner = m_Device.Scanner;
            ret.Device.FingerVeinner = m_Device.FingerVeinner;
            ret.Device.Im.IsUse = m_Device.ImIsUse;
            ret.Device.Im.Partner = m_Device.ImPartner;
            ret.Device.Im.UserName = m_Device.ImUserName;
            ret.Device.Im.Password = m_Device.ImPassword;

            ret.Device.Mqtt = new MqttModel
            {
                Type = "exmq",
                Params = new
                {
                    Host = "tcp://112.74.179.185:1883",
                    UserName = "admin",
                    Password = "public",
                    ClientId = m_Device.DeviceId,
                    DeviceClass = "a1A2Mq6w5ln"
                }
            };

            ret.Device.PicInSampleSize = 8;

            if (m_Device.Type == "vending")
            {
                ret.CustomData = new
                {
                    MaxBuyNumber = 10,
                    IsHiddenKind = m_Device.KindIsHidden,
                    KindRowCellSize = m_Device.KindRowCellSize,
                    Ads = BizFactory.Device.GetAds(m_Device.DeviceId),
                    Kinds = GetKinds(m_Device.MerchId, m_Device.StoreId, m_Device.ShopId, m_Device.DeviceId),
                    Skus = GetSkus(m_Device.MerchId, m_Device.StoreId, m_Device.ShopId, m_Device.DeviceId)
                };
            }
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public Dictionary<string, SkuModel> GetSkus(string merchId, string storeId, string shopId, string deviceId)
        {
            var l_Skus = StoreTermServiceFactory.Product.GetSkus(0, int.MaxValue, merchId, storeId, shopId, deviceId);

            var dics = new Dictionary<string, SkuModel>();

            if (l_Skus == null)
            {
                return dics;
            }

            if (l_Skus.Items == null)
            {
                return dics;
            }

            if (l_Skus.Items.Count == 0)
            {
                return dics;
            }

            foreach (var item in l_Skus.Items)
            {
                dics.Add(item.SkuId, item);
            }

            return dics;
        }

        public List<KindModel> GetKinds(string merchId, string storeId, string shopId, string deviceId)
        {
            var l_kinds = new List<KindModel>();

            var d_kinds = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            var d_stocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Device && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.DeviceId == deviceId).ToList();

            var l_kind_all = new KindModel();
            l_kind_all.KindId = IdWorker.Build(IdType.EmptyGuid);
            l_kind_all.Name = "全部";
            l_kind_all.Childs = d_stocks.Select(m => m.SkuId).Distinct().ToList();

            l_kinds.Add(l_kind_all);

            foreach (var d_kind in d_kinds)
            {
                var l_kind = new KindModel();
                l_kind.KindId = d_kind.Id;
                l_kind.Name = d_kind.Name;

                var l_SpuIds = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.StoreKindId == d_kind.Id).Select(m => m.SpuId).Distinct().ToList();
                if (l_SpuIds.Count > 0)
                {
                    var l_SkuIds = d_stocks.Where(m => l_SpuIds.Contains(m.SpuId)).Select(m => m.SkuId).Distinct().ToList();
                    if (l_SpuIds.Count > 0)
                    {
                        l_kind.Childs = l_SkuIds;
                        l_kinds.Add(l_kind);
                    }
                }
            }

            return l_kinds;
        }

        public IResult CheckUpdate(RopDeviceCheckUpdate rup)
        {
            CustomJsonResult result = new CustomJsonResult();

            var d_appSoftware = CurrentDb.AppSoftware.Where(m => m.AppId == rup.AppId && m.AppKey == rup.AppKey).FirstOrDefault();
            if (d_appSoftware == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            if (string.IsNullOrEmpty(d_appSoftware.VersionName))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            if (string.IsNullOrEmpty(d_appSoftware.DownloadUrl))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }

            var model = new { versionCode = d_appSoftware.VersionCode, versionName = d_appSoftware.VersionName, downloadUrl = d_appSoftware.DownloadUrl };

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", model);
        }

        public IResult EventNotify(string operater, RopDeviceEventNotify rop)
        {
            BizFactory.Device.EventNotify(operater, AppId.STORETERM, rop.DeviceId, rop.EventCode, rop.EventRemark, rop.Content);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }

        public IResult GetRunExHandleItems(string operater, RopDeviceGetRunExHandleItems rop)
        {
            var result = new CustomJsonResult();

            var ret = new RetDeviceGetRunExHandleItems();

            ret.ExReasons.Add(new RetDeviceGetRunExHandleItems.ExReason { ReasonId = "1", Title = "App异常退出" });
            ret.ExReasons.Add(new RetDeviceGetRunExHandleItems.ExReason { ReasonId = "2", Title = "设备出现故障" });
            ret.ExReasons.Add(new RetDeviceGetRunExHandleItems.ExReason { ReasonId = "3", Title = "未知原因" });

            var d_orders = CurrentDb.Order.Where(m => m.DeviceId == rop.DeviceId && m.ExIsHappen == true && m.ExIsHandle == false).ToList();

            foreach (var d_order in d_orders)
            {
                var l_exItem = new RetDeviceGetRunExHandleItems.ExItem();

                l_exItem.ItemId = d_order.Id;

                var d_orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_order.Id).ToList();

                foreach (var d_orderSub in d_orderSubs)
                {
                    var l_exUnique = new RetDeviceGetRunExHandleItems.ExUnique();
                    l_exUnique.UniqueId = d_orderSub.Id;
                    l_exUnique.SkuId = d_orderSub.SkuId;
                    l_exUnique.SlotId = d_orderSub.SlotId;
                    l_exUnique.Quantity = d_orderSub.Quantity;
                    l_exUnique.Name = d_orderSub.SkuName;
                    l_exUnique.MainImgUrl = d_orderSub.SkuMainImgUrl;
                    l_exUnique.Status = BizFactory.Order.GetPickupStatus(d_orderSub.PickupStatus);
                    l_exUnique.SignStatus = 0;
                    if (d_orderSub.PickupStatus == E_OrderPickupStatus.Taked || d_orderSub.PickupStatus == E_OrderPickupStatus.ExPickupSignUnTaked && d_orderSub.PickupStatus == E_OrderPickupStatus.ExPickupSignTaked)
                    {
                        l_exUnique.CanHandle = false;
                    }
                    else
                    {
                        l_exUnique.CanHandle = true;
                    }

                    l_exItem.Uniques.Add(l_exUnique);
                }

                ret.ExItems.Add(l_exItem);
            }


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public IResult HandleRunExItems(string operater, RopDeviceHandleRunExItems rop)
        {
            var result = new CustomJsonResult();

            var bizRop = new RopOrderHandleExByDeviceSelfTake();
            bizRop.IsRunning = true;
            bizRop.Remark = string.Join(",", rop.ExReasons.Select(m => m.Title).ToArray());
            bizRop.Items = rop.ExItems;
            bizRop.DeviceId = rop.DeviceId;
            bizRop.MerchId = rop.MerchId;
            bizRop.AppId = rop.AppId;
            result = BizFactory.Order.HandleExByDeviceSelfTake(operater, bizRop);

            return result;
        }
    }
}
