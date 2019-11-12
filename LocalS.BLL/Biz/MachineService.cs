﻿using MyPushSdk;
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
            model.CabineRowColLayout_1 = GetLayout(machine.CabinetRowColLayout_1);
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

        public void SendUpdateProductSkuStock(string id, List<UpdateProductSkuStockModel> productSkus)
        {
            if (productSkus != null && productSkus.Count > 0)
            {
                var machine = BizFactory.Machine.GetOne(id);
                PushService.SendUpdateProductSkuStock(machine.JPushRegId, productSkus);
            }
        }

        public void SendUpdateHomeBanner(string id)
        {
            //if (homeBanners != null && homeBanners.Count > 0)
            //{
            //    var machine = BizFactory.Machine.GetOne(id);
            //    PushService.SendUpdateHomeBanner(machine.JPushRegId, homeBanners);
            //}
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
