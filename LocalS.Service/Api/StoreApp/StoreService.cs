using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeiXinSdk;

namespace LocalS.Service.Api.StoreApp
{
    public class StoreService : BaseDbContext
    {
        public CustomJsonResult List(string operater, string clientUserId, RupStoreList rup)
        {
            var result = new CustomJsonResult();

            var stores = CurrentDb.Store.Where(m => m.MerchId == rup.MerchId && m.IsDelete == false).ToList();

            var storeModels = new List<StoreModel>();
            foreach (var m in stores)
            {
                double distance = 0;
                string distanceMsg = "";

                if (rup.Lat == 0 || rup.Lng == 0)
                {
                    distanceMsg = "";
                }
                else
                {
                    distance = DistanceUtil.GetDistance(m.Lat, m.Lng, rup.Lat, rup.Lng);

                    distanceMsg = string.Format("{0}km", distance.ToString("f2"));
                }

                storeModels.Add(new StoreModel { Id = m.Id, Name = m.Name, Address = m.Address, Distance = distance, DistanceMsg = distanceMsg });
            }

            storeModels = storeModels.OrderBy(m => m.Distance).ToList();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", storeModels);

            return result;
        }
    }
}
