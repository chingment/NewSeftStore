using LocalS.BLL;
using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class SearchService
    {
        public CustomJsonResult TobeSearch(string operater, string clientUserId, RupSearchTobeSearch rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetSearchTobeSearch();

            var store = BizFactory.Store.GetOne(rup.StoreId);

            if (store == null)
            {
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "机器未登记");
            }


            ret.ProductSkus = CacheServiceFactory.Product.SearchSku(store.MerchId, "All", rup.Key);

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);

            return result;
        }
    }
}
