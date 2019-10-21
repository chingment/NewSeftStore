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
            model.LogoImgUrl = machine.LogoImgUrl;
            model.JPushRegId = machine.JPushRegId;
            model.CabinetId_1 = machine.CabinetId_1;
            model.CabinetName_1 = machine.CabinetName_1;
            model.CabinetMaxRow_1 = machine.CabinetMaxRow_1;
            model.CabinetMaxCol_1 = machine.CabinetMaxCol_1;

            var merch = CurrentDb.Merch.Where(m => m.Id == machine.MerchId).FirstOrDefault();

            if (merch != null)
            {
                model.MerchId = merch.Id;
                model.MerchName = merch.Name;
                model.CsrQrCode = merch.CsrQrCode;

                var merchMachine = CurrentDb.MerchMachine.Where(m => m.MerchId == machine.MerchId && m.MachineId == id).FirstOrDefault();
                if (merchMachine != null)
                {
                    model.Name = merchMachine.Name;
                    model.LogoImgUrl = merchMachine.LogoImgUrl;
                }

                var merchStore = BizFactory.Store.GetOne(machine.StoreId);
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
    }
}
