using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.BLL.UI;
using Lumos;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class CommonService : BaseService
    {
        public CustomJsonResult GetStores(string operater, string merchId)
        {
            var result = new CustomJsonResult();

            var ret = new List<object>();

            var d_Stores = CurrentDb.Store.Where(m => m.MerchId == merchId && m.IsDelete == false).OrderByDescending(r => r.CreateTime).ToList();

            foreach (var d_Store in d_Stores)
            {
                ret.Add(new { Value = d_Store.Id, Label = d_Store.Name });
            }

            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
        }
    }
}
