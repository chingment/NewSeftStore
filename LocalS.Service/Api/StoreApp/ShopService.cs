using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyWeiXinSdk;
using LocalS.BLL.Biz;

namespace LocalS.Service.Api.StoreApp
{
    public class ShopService : BaseService
    {
        public CustomJsonResult List(string operater, string clientUserId, RupShopList rup)
        {
            var result = new CustomJsonResult();

            LogUtil.Info("rup:" + rup.ToJsonString());

            var d_Shops = (from s in CurrentDb.StoreShop
                           join m in CurrentDb.Shop on s.ShopId equals m.Id into temp
                           from u in temp.DefaultIfEmpty()
                           where
                           s.MerchId == rup.MerchId
                           && s.StoreId == rup.StoreId
                           select new { u.Id, u.Name, u.Address, u.Lat, u.Lng, u.MainImgUrl, u.IsOpen, u.AreaCode, u.AreaName, u.MerchId, s.StoreId, u.ContactName, u.ContactPhone, u.ContactAddress, u.CreateTime }).ToList();

            var storeModels = new List<StoreModel>();

            foreach (var d_Shop in d_Shops)
            {
                double distance = 0;
                string distanceMsg = "";

                if (rup.Lat == 0 || rup.Lng == 0)
                {
                    distanceMsg = "";
                }
                else
                {
                    distance = DistanceUtil.GetDistance(d_Shop.Lat, d_Shop.Lng, rup.Lat, rup.Lng);

                    distanceMsg = string.Format("{0}km", distance.ToString("f2"));
                }

                storeModels.Add(new StoreModel { Id = d_Shop.Id, Name = d_Shop.Name, Address = d_Shop.Address, Distance = distance, DistanceMsg = distanceMsg });
            }

            storeModels = storeModels.OrderBy(m => m.Distance).ToList();


            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", storeModels);

            return result;
        }
    }
}
