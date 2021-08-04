using LocalS.Service.Api.IotTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace WebApiIotTerm.Controllers
{
    public class StockController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse RelienishPlans(RopStockRelienishPlans rop)
        {
            var result = IotTermServiceFactory.Stock.RelienishPlans(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RelienishPlanDetails(RopStockRelienishPlanDetails rop)
        {
            var result = IotTermServiceFactory.Stock.RelienishPlanDetails(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }
    }
}
