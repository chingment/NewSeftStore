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

            model.Id = store.Id;
            model.Name = store.Name;
            model.MerchId = store.MerchId;
            model.Address = store.Address;
            model.BriefDes = store.BriefDes;
            model.DisplayImgUrls = store.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
            model.Lng = store.Lng;
            model.Lat = store.Lat;
            model.IsOpen = store.IsOpen;
            model.IsDelete = store.IsDelete;
            model.MachineIds = CurrentDb.MerchMachine.Where(m => m.StoreId == id).Select(m => m.MachineId).ToArray();
            return model;
        }


        public List<StoreInfoModel> GetAll(string merchId)
        {
            var models = new List<StoreInfoModel>();

            var stores = CurrentDb.Store.Where(m => m.MerchId == merchId).ToList();


            foreach (var store in stores)
            {
                var model = new StoreInfoModel();
                model.Id = store.Id;
                model.Name = store.Name;
                model.MerchId = store.MerchId;
                model.Address = store.Address;
                model.BriefDes = store.BriefDes;
                model.DisplayImgUrls = store.DisplayImgUrls.ToJsonObject<List<ImgSet>>();
                model.Lng = store.Lng;
                model.Lat = store.Lat;
                model.IsOpen = store.IsOpen;
                model.IsDelete = store.IsDelete;

                models.Add(model);

            }


            return models;
        }
    }
}
