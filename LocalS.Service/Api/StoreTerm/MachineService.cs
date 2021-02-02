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
    public class MachineService : BaseService
    {
        public CustomJsonResult InitData(RopMachineInitData rop)
        {
            CustomJsonResult result = new CustomJsonResult();

            var ret = new RetMachineInitData();

            if (string.IsNullOrEmpty(rop.DeviceId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "设备编码为空");
            }

            var d_machine = CurrentDb.Machine.Where(m => m.ImeiId == rop.DeviceId || m.MacAddress == rop.DeviceId).FirstOrDefault();

            if (d_machine == null)
            {
                d_machine = new Machine();
                d_machine.Id = IdWorker.Build(IdType.MachineId);
                d_machine.Name = "贩卖X1";//默认名称
                d_machine.JPushRegId = rop.JPushRegId;
                d_machine.DeviceId = rop.DeviceId;
                d_machine.ImeiId = string.IsNullOrEmpty(rop.ImeiId) == true ? IdWorker.Build(IdType.NewGuid) : rop.ImeiId;
                d_machine.MacAddress = string.IsNullOrEmpty(rop.MacAddress) == true ? IdWorker.Build(IdType.NewGuid) : rop.MacAddress;
                d_machine.MainImgUrl = "http://file.17fanju.com/Upload/machine1.jpg";
                d_machine.AppVersionCode = rop.AppVersionCode;
                d_machine.AppVersionName = rop.AppVersionName;
                d_machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                d_machine.KindIsHidden = false;
                d_machine.KindRowCellSize = 3;
                d_machine.CreateTime = DateTime.Now;
                d_machine.Creator = IdWorker.Build(IdType.EmptyGuid);
                CurrentDb.Machine.Add(d_machine);
                CurrentDb.SaveChanges();
            }
            else
            {
                d_machine.JPushRegId = rop.JPushRegId;
                d_machine.AppVersionCode = rop.AppVersionCode;
                d_machine.AppVersionName = rop.AppVersionName;
                d_machine.CtrlSdkVersionCode = rop.CtrlSdkVersionCode;
                d_machine.MendTime = DateTime.Now;
                d_machine.Mender = IdWorker.Build(IdType.EmptyGuid);
                CurrentDb.SaveChanges();
            }

            if (string.IsNullOrEmpty(d_machine.CurUseMerchId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定商户");
            }

            if (string.IsNullOrEmpty(d_machine.CurUseStoreId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定店铺");
            }

            if (string.IsNullOrEmpty(d_machine.CurUseShopId))
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未绑定门店");
            }

            var l_machine = BizFactory.Machine.GetOne(d_machine.Id);
            ret.Machine.MachineId = l_machine.MachineId;
            ret.Machine.DeviceId = l_machine.DeviceId;
            ret.Machine.Name = l_machine.Name;
            ret.Machine.LogoImgUrl = l_machine.LogoImgUrl;
            ret.Machine.MerchName = l_machine.MerchName;
            ret.Machine.StoreName = l_machine.StoreName;
            ret.Machine.ShopName = l_machine.ShopName;
            ret.Machine.ShopAddress = l_machine.ShopAddress;
            ret.Machine.CsrQrCode = l_machine.CsrQrCode;
            ret.Machine.CsrPhoneNumber = l_machine.CsrPhoneNumber;
            ret.Machine.CsrHelpTip = l_machine.CsrHelpTip;
            ret.Machine.Cabinets = l_machine.Cabinets;
            ret.Machine.IsHiddenKind = l_machine.KindIsHidden;
            ret.Machine.KindRowCellSize = l_machine.KindRowCellSize;
            ret.Machine.PayOptions = l_machine.PayOptions;
            ret.Machine.CameraByChkIsUse = l_machine.CameraByChkIsUse;
            ret.Machine.CameraByJgIsUse = l_machine.CameraByJgIsUse;
            ret.Machine.CameraByRlIsUse = l_machine.CameraByRlIsUse;
            ret.Machine.MaxBuyNumber = 10;
            ret.Machine.ExIsHas = l_machine.ExIsHas;
            ret.Machine.OstVern = l_machine.OstVern;
            ret.Machine.MstVern = l_machine.MstVern;
            ret.Machine.Scanner = l_machine.Scanner;
            ret.Machine.FingerVeinner = l_machine.FingerVeinner;
            ret.Machine.Im.IsUse = l_machine.ImIsUse;
            ret.Machine.Im.Partner = l_machine.ImPartner;
            ret.Machine.Im.UserName = l_machine.ImUserName;
            ret.Machine.Im.Password = l_machine.ImPassword;
            ret.Machine.Mqtt.Host = "tcp://112.74.179.185:1883";
            ret.Machine.Mqtt.UserName = "admin";
            ret.Machine.Mqtt.Password = "public";
            ret.Machine.PicInSampleSize = 8;

            ret.Ads = BizFactory.Machine.GetAds(l_machine.MachineId);
            ret.ProductKinds = StoreTermServiceFactory.Machine.GetProductKinds(l_machine.MerchId, l_machine.StoreId, l_machine.ShopId, l_machine.MachineId);
            ret.ProductSkus = StoreTermServiceFactory.Machine.GetProductSkus(l_machine.MerchId, l_machine.StoreId, l_machine.ShopId, l_machine.MachineId);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }

        public Dictionary<string, ProductSkuModel> GetProductSkus(string merchId, string storeId, string shopId, string machineId)
        {
            var l_products = StoreTermServiceFactory.ProductSku.GetPageList(0, int.MaxValue, merchId, storeId, shopId, machineId);

            var dics = new Dictionary<string, ProductSkuModel>();

            if (l_products == null)
            {
                return dics;
            }

            if (l_products.Items == null)
            {
                return dics;
            }

            if (l_products.Items.Count == 0)
            {
                return dics;
            }

            foreach (var item in l_products.Items)
            {
                dics.Add(item.ProductSkuId, item);
            }

            return dics;
        }

        public List<ProductKindModel> GetProductKinds(string merchId, string storeId, string shopId, string machineId)
        {
            var l_kinds = new List<ProductKindModel>();

            var d_kinds = CurrentDb.StoreKind.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.IsDelete == false).OrderBy(m => m.Priority).ToList();

            var d_stocks = CurrentDb.SellChannelStock.Where(m => m.ShopMode == E_ShopMode.Machine && m.MerchId == merchId && m.StoreId == storeId && m.ShopId == shopId && m.MachineId == machineId).ToList();

            var l_kind_all = new ProductKindModel();
            l_kind_all.KindId = IdWorker.Build(IdType.EmptyGuid);
            l_kind_all.Name = "全部";
            l_kind_all.Childs = d_stocks.Select(m => m.PrdProductSkuId).Distinct().ToList();

            l_kinds.Add(l_kind_all);

            foreach (var d_kind in d_kinds)
            {
                var l_kind = new ProductKindModel();
                l_kind.KindId = d_kind.Id;
                l_kind.Name = d_kind.Name;

                var l_productIds = CurrentDb.StoreKindSpu.Where(m => m.MerchId == merchId && m.StoreId == storeId && m.StoreKindId == d_kind.Id).Select(m => m.PrdProductId).Distinct().ToList();
                if (l_productIds.Count > 0)
                {
                    var l_productSkuIds = d_stocks.Where(m => l_productIds.Contains(m.PrdProductId)).Select(m => m.PrdProductSkuId).Distinct().ToList();
                    if (l_productIds.Count > 0)
                    {
                        l_kind.Childs = l_productSkuIds;
                        l_kinds.Add(l_kind);
                    }
                }
            }

            return l_kinds;
        }

        public CustomJsonResult CheckUpdate(RupMachineCheckUpdate rup)
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

        public CustomJsonResult EventNotify(string operater, RopMachineEventNotify rop)
        {
            BizFactory.Machine.EventNotify(operater, AppId.STORETERM, rop.MachineId, rop.EventCode, rop.EventRemark, rop.Content);

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }

        public CustomJsonResult GetRunExHandleItems(string operater, RupMachineGetRunExHandleItems rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetMachineGetRunExHandleItems();

            ret.ExReasons.Add(new RetMachineGetRunExHandleItems.ExReason { ReasonId = "1", Title = "App异常退出" });
            ret.ExReasons.Add(new RetMachineGetRunExHandleItems.ExReason { ReasonId = "2", Title = "机器出现故障" });
            ret.ExReasons.Add(new RetMachineGetRunExHandleItems.ExReason { ReasonId = "3", Title = "未知原因" });

            var d_orders = CurrentDb.Order.Where(m => m.MachineId == rup.MachineId && m.ExIsHappen == true && m.ExIsHandle == false).ToList();

            foreach (var d_order in d_orders)
            {
                var l_exItem = new RetMachineGetRunExHandleItems.ExItem();

                l_exItem.ItemId = d_order.Id;

                var d_orderSubs = CurrentDb.OrderSub.Where(m => m.OrderId == d_order.Id).ToList();

                foreach (var d_orderSub in d_orderSubs)
                {
                    var l_exUnique = new RetMachineGetRunExHandleItems.ExUnique();
                    l_exUnique.UniqueId = d_orderSub.Id;
                    l_exUnique.ProductSkuId = d_orderSub.PrdProductSkuId;
                    l_exUnique.SlotId = d_orderSub.SlotId;
                    l_exUnique.Quantity = d_orderSub.Quantity;
                    l_exUnique.Name = d_orderSub.PrdProductSkuName;
                    l_exUnique.MainImgUrl = d_orderSub.PrdProductSkuMainImgUrl;
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

        public CustomJsonResult HandleRunExItems(string operater, RopMachineHandleRunExItems rop)
        {
            var result = new CustomJsonResult();

            var bizRop = new RopOrderHandleExByMachineSelfTake();
            bizRop.IsRunning = true;
            bizRop.Remark = string.Join(",", rop.ExReasons.Select(m => m.Title).ToArray());
            bizRop.Items = rop.ExItems;
            bizRop.MachineId = rop.MachineId;
            bizRop.MerchId = rop.MerchId;
            bizRop.AppId = rop.AppId;
            result = BizFactory.Order.HandleExByMachineSelfTake(operater, bizRop);

            return result;
        }
    }
}
