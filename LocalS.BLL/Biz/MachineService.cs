using LocalS.Entity;
using MyPushSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            model.Name = machine.Name;
            model.MainImgUrl = machine.MainImgUrl;
            model.LogoImgUrl = machine.LogoImgUrl;
            model.JPushRegId = machine.JPushRegId;
            model.CabinetId_1 = machine.CabinetId_1;
            model.CabinetName_1 = machine.CabinetName_1;
            model.CabinetRowColLayout_1 = GetLayout(machine.CabinetRowColLayout_1);
            model.RunStatus = machine.RunStatus;
            model.LastRequestTime = machine.LastRequestTime;
            model.AppVersion = machine.AppVersionName;
            model.CtrlSdkVersion = machine.CtrlSdkVersionCode;

            var merch = CurrentDb.Merch.Where(m => m.Id == machine.CurUseMerchId).FirstOrDefault();

            if (merch != null)
            {
                model.MerchId = merch.Id;
                model.MerchName = merch.Name;
                model.CsrQrCode = merch.CsrQrCode;

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

            var adContents = CurrentDb.AdContent.Where(m => adContentIds.Contains(m.Id) && m.Status == E_AdContentStatus.Normal).ToList();


            foreach (var item in adContents)
            {
                bannerModels.Add(new BannerModel { Url = item.Url });
            }

            return bannerModels;
        }

        public void SendUpdateStockSlots(string id, List<UpdateMachineStockSlotModel> stockSlots)
        {
            if (stockSlots != null && stockSlots.Count > 0)
            {
                var machine = BizFactory.Machine.GetOne(id);
                PushService.SendUpdateMachineStockSlots(machine.JPushRegId, stockSlots);
            }
        }

        public void SendUpdateHomeBanners(string id)
        {
            var machine = BizFactory.Machine.GetOne(id);
            var banners = BizFactory.Machine.GetHomeBanners(id);
            PushService.SendUpdateMachineHomeBanners(machine.JPushRegId, banners);
        }

        private static int[] GetLayout(string str)
        {
            int[] layout = null;

            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                string[] data = str.Split(',');
                if (data.Length > 0)
                {

                    layout = new int[data.Length];

                    for (int i = 0; i < data.Length; i++)
                    {
                        layout[i] = int.Parse(data[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }


            return layout;
        }
    }
}
