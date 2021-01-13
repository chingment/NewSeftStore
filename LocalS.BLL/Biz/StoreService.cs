using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class StoreService : BaseService
    {
        public StoreInfoModel GetOne(string id)
        {
            var model = new StoreInfoModel();

            var store = CurrentDb.Store.Where(m => m.Id == id).FirstOrDefault();

            if (store == null)
                return null;

            var merch = CurrentDb.Merch.Where(m => m.Id == store.MerchId).FirstOrDefault();

            model.StoreId = store.Id;
            model.Name = store.Name;
            model.MerchId = store.MerchId;
            model.MerchName = merch.Name;
            model.BriefDes = store.BriefDes;
            model.IsDelete = store.IsDelete;
            model.IsTestMode = store.IsTestMode;
            model.SctMode = store.SctMode;
            return model;
        }
    }
}
