using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class GlobalService : BaseDbContext
    {
        public CustomJsonResult DataSet(string operater, string clientUserId, RupGlobalDataSet rup)
        {

            var result = new CustomJsonResult();
            var ret = new RetGobalDataSet();

            ret.Index = StoreAppServiceFactory.Index.GetPageData(operater, clientUserId, rup.StoreId).Data;
            ret.ProductKind = StoreAppServiceFactory.ProductKind.GetPageData(operater, clientUserId, rup.StoreId).Data;
            ret.Cart = StoreAppServiceFactory.Cart.GetPageData(operater, clientUserId, new RupCartPageData { StoreId= rup.StoreId }).Data;
            ret.Personal = StoreAppServiceFactory.Personal.GetPageData(operater, clientUserId, rup.StoreId).Data;

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);


            return result;
        }

    }
}
