using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class SaleOutletService : BaseService
    {
        public CustomJsonResult List(string operater, string clientUserId, RupSaleOutletList rup)
        {
            var result = new CustomJsonResult();

            var d_Store = CurrentDb.Store.Where(m => m.Id == rup.StoreId).FirstOrDefault();

            var query = (from u in CurrentDb.SaleOutlet
                         where
u.MerchId == d_Store.MerchId
                         select new { u.Id, u.Name, u.ContactName, u.ContactAddress, u.ContactPhone, u.CreateTime });

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

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", new { saleOutlets = olist });

            return result;
        }
    }
}
