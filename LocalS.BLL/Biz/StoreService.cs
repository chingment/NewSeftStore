using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class StoreService : BaseDbContext
    {


        public StoreInfoModel GetOne(string id)
        {
            var model = new StoreInfoModel();

            var store = CurrentDb.Store.Where(m => m.Id == id).FirstOrDefault();

            if (store == null)
                return null;

            var merch = CurrentDb.Merch.Where(m => m.Id == store.MerchId).FirstOrDefault();

            var merchMachines = CurrentDb.MerchMachine.Where(m => m.CurUseStoreId == id).ToList();

            model.StoreId = store.Id;
            model.Name = store.Name;
            model.MerchId = store.MerchId;
            model.MerchName = merch.Name;
            model.Address = store.Address;
            model.BriefDes = store.BriefDes;
            model.DisplayImgUrls = store.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
            model.AddressPoint.Lng = store.Lng;
            model.AddressPoint.Lat = store.Lat;
            model.IsOpen = store.IsOpen;
            model.IsDelete = store.IsDelete;
            model.AllMachineIds = merchMachines.Select(m => m.MachineId).ToArray();
            model.SellMachineIds = merchMachines.Where(m => m.IsStopUse == false).Select(m => m.MachineId).ToArray();
            model.IsTestMode = store.IsTestMode;
            return model;
        }

        public List<StoreInfoModel> GetAll(string merchId)
        {
            var models = new List<StoreInfoModel>();

            var stores = CurrentDb.Store.Where(m => m.MerchId == merchId).OrderByDescending(r => r.CreateTime).ToList();


            foreach (var store in stores)
            {
                var merchMachines = CurrentDb.MerchMachine.Where(m => m.CurUseStoreId == store.Id).ToList();
                var model = new StoreInfoModel();
                model.StoreId = store.Id;
                model.Name = store.Name;
                model.MerchId = store.MerchId;
                model.Address = store.Address;
                model.BriefDes = store.BriefDes;
                model.DisplayImgUrls = store.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                model.AddressPoint.Lat = store.Lat;
                model.AddressPoint.Lng = store.Lng;
                model.IsOpen = store.IsOpen;
                model.IsDelete = store.IsDelete;
                model.AllMachineIds = merchMachines.Select(m => m.MachineId).ToArray();
                model.SellMachineIds = merchMachines.Where(m => m.IsStopUse == false).Select(m => m.MachineId).ToArray();
                model.IsTestMode = store.IsTestMode;
                models.Add(model);

            }


            return models;
        }
    }
}
