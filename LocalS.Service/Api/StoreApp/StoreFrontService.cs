using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LocalS.Service.Api.StoreApp
{
    public class StoreFrontService : BaseService
    {

        public CustomJsonResult List(string operater, string clientUserId, RupSelfPickAddressList rup)
        {
            var result = new CustomJsonResult();

            var query = (from s in CurrentDb.StoreShop
                                   join m in CurrentDb.Shop on s.ShopId equals m.Id into temp
                                   from u in temp.DefaultIfEmpty()
                                   where
                             u.MerchId == rup.MerchId
                                 && s.StoreId == rup.StoreId
                                   select new { u.Id, u.Name, u.Address, u.MainImgUrl, u.IsOpen, u.AreaCode, u.AreaName, u.MerchId, s.StoreId, u.ContactName, u.ContactPhone, u.ContactAddress, u.CreateTime });


            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    Address = item.Address,
                    AreaCode = item.AreaCode,
                    AreaName = item.AreaName,
                    ContactName = item.ContactName,
                    ContactAddress = item.ContactAddress,
                    ContactPhone = item.ContactPhone
                });
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { selfPickAddress = olist });

            return result;
        }
    }
}
