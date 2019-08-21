using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class StoreService : BaseDbContext
    {
        public List<StoreModel> List(string operater, string clientId, RupStoreList rup)
        {

            var stores = CurrentDb.Store.Where(m => m.IsClose == false).ToList();

            if (!string.IsNullOrEmpty(rup.MerchId))
            {
                stores = stores.Where(m => m.MerchId == rup.MerchId).ToList();
            }

            var storeModels = new List<StoreModel>();
            foreach (var m in stores)
            {
                double distance = 0;
                string distanceMsg = "";

                distance = DistanceUtil.GetDistance(m.Lat, m.Lng, rup.Lat, rup.Lng);

                distanceMsg = string.Format("{0}km", distance.ToString("f2"));

                storeModels.Add(new StoreModel { Id = m.Id, Name = m.Name, Address = m.Address, DistanceMsg = distanceMsg });
            }

            return storeModels;
        }
    }
}
