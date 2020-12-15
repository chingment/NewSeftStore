using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class SupplierService : BaseDbContext
    {
        public CustomJsonResult Search(string operater, string merchId, string key)
        {
            var query = (from u in CurrentDb.Supplier
                         where 
                       
                         u.MerchId == merchId
                         select new { u.Id, u.Name, u.CumCode, u.CreateTime });


            int total = query.Count();
            int pageIndex = 0;
            int pageSize = int.MaxValue;

            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    Name = item.Name,
                    CumCode = item.CumCode
                });
            }

            PageEntity pageEntity = new PageEntity { PageSize = pageSize, Total = total, Items = olist };


            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", pageEntity);

        }
    }
}
