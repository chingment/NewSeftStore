using LocalS.BLL.Mq;
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

            model.Id = machine.Id;
            model.DeviceId = machine.DeviceId;
            model.Name = machine.Name;
            model.MainImgUrl = machine.MainImgUrl;
            model.LogoImgUrl = machine.LogoImgUrl;
            model.JPushRegId = machine.JPushRegId;
            model.RunStatus = machine.RunStatus;
            model.LastRequestTime = machine.LastRequestTime;
            model.AppVersion = machine.AppVersionName;
            model.CtrlSdkVersion = machine.CtrlSdkVersionCode;
            model.IsHiddenKind = machine.IsHiddenKind;
            model.KindRowCellSize = machine.KindRowCellSize;
            model.IsTestMode = machine.IsTestMode;
            model.IsOpenChkCamera = machine.IsOpenChkCamera;
            model.ExIsHas = machine.ExIsHas;

            var machineCabinets = CurrentDb.MachineCabinet.Where(m => m.MachineId == id && m.IsUse == true).OrderByDescending(m => m.Priority).ToList();

            foreach (var machineCabinet in machineCabinets)
            {
                var cabinet = new CabinetInfoModel();
                cabinet.Id = machineCabinet.CabinetId;
                cabinet.Name = machineCabinet.CabinetName;
                cabinet.RowColLayout = machineCabinet.RowColLayout;
                cabinet.Priority = machineCabinet.Priority;
                cabinet.FixSlotQuantity = machineCabinet.FixSlotQuantity;
                cabinet.ComId = machineCabinet.ComId;
                model.Cabinets.Add(cabinet.Id, cabinet);
            }

            if (machine.IsUseFingerVeinCtrl)
            {
                model.FingerVeinCtrl.IsUse = true;
            }

            if (machine.IsUseSanCtrl)
            {
                model.ScanCtrl.IsUse = true;
                model.ScanCtrl.ComId = machine.SanCtrlComId;
            }


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
                    model.StoreId = merchStore.Id;
                    model.StoreName = merchStore.Name;
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

        public void SendUpdateProductSkuStock(string id, UpdateMachineProdcutSkuStockModel updateProdcutSkuStock)
        {
            if (updateProdcutSkuStock != null)
            {
                var machine = BizFactory.Machine.GetOne(id);
                PushService.SendUpdateProductSkuStock(machine.JPushRegId, updateProdcutSkuStock);
            }
        }

        public void SendUpdateHomeBanners(string id)
        {
            var machine = BizFactory.Machine.GetOne(id);
            var banners = BizFactory.Machine.GetHomeBanners(id);
            PushService.SendUpdateMachineHomeBanners(machine.JPushRegId, banners);
        }

        public void SendUpdateHomeLogo(string id, string logoImgUrl)
        {
            var machine = BizFactory.Machine.GetOne(id);
            var content = new { url = logoImgUrl };
            PushService.SendUpdateMachineHomeLogo(machine.JPushRegId, content);
        }

        public void SendPaySuccess(string id, string orderId, string orderSn)
        {
            //var machine = BizFactory.Machine.GetOne(id);
            //var orderDetails = BizFactory.Order.GetOrderDetailsByPickup(orderId, id);
            //var content = new { orderId = orderId, orderSn = orderSn, status = E_OrderStatus.Payed, OrderDetails = orderDetails };
            //PushService.SendPaySuccess(machine.JPushRegId, content);
        }

        public CustomJsonResult EventNotify(string operater, string appId, string machineId, string eventCode, object content)
        {
            var machine = BizFactory.Machine.GetOne(machineId);
            MqFactory.Global.PushEventNotify(operater, appId, machine.MerchId, machine.StoreId, machineId, eventCode, "", content);
            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        }
    }
}
