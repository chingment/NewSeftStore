using LocalS.BLL;
using LocalS.Entity;
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

            ret.Index = StoreAppServiceFactory.Index.PageData(operater, clientUserId, new RupIndexPageData { StoreId = rup.StoreId }).Data;
            ret.ProductKind = StoreAppServiceFactory.ProductKind.PageData(operater, clientUserId, new RupProductKindPageData { StoreId = rup.StoreId }).Data;
            ret.Cart = StoreAppServiceFactory.Cart.PageData(operater, clientUserId, new RupCartPageData { StoreId = rup.StoreId }).Data;
            ret.Personal = StoreAppServiceFactory.Personal.PageData(operater, clientUserId, new RupPersonalPageData { StoreId = rup.StoreId }).Data;

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);


            return result;
        }


        public CustomJsonResult MsgTips(string operater, string clientUserId, RupGlobalMsgTips rup)
        {
            var result = new CustomJsonResult();

            var ret = new RetGlobalMsgTips();

            var d_clientCart_Quantity = CurrentDb.ClientCart.Where(m => m.ClientUserId == clientUserId && m.StoreId == rup.StoreId && m.Status == E_ClientCartStatus.WaitSettle).Sum(m => m.Quantity);

            ret.BadgeByCart = new UI.Badge { Type = "number", Value = d_clientCart_Quantity.ToString() };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return result;
        }

    }
}
