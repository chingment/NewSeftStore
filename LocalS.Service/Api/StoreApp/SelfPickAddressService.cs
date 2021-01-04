using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LocalS.Service.Api.StoreApp
{
    public class SelfPickAddressService : BaseService
    {

        public CustomJsonResult List(string operater, string clientUserId, RupSelfPickAddressList rup)
        {
            var result = new CustomJsonResult();

            var query = (from u in CurrentDb.StoreSelfPickAddress
                         join m in CurrentDb.SelfPickAddress on u.SelfPickAddressId equals m.Id into temp
                         from tt in temp.DefaultIfEmpty()
                         where u.MerchId == rup.MerchId && u.StoreId == rup.StoreId
                         select new { tt.Id, tt.Name, u.MerchId, u.StoreId, tt.ContactName, tt.ContactPhone, tt.ContactAddress });


            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
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
