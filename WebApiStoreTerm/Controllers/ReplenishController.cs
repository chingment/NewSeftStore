using LocalS.BLL.Biz;
using LocalS.Service.Api.StoreTerm;
using Lumos;
using System.IO;
using System.Web;
using System.Web.Http;

namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class ReplenishController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse GetPlans([FromBody]RopReplenishGetPlans rop)
        {
            var result = StoreTermServiceFactory.Replenish.GetPlans(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse GetPlanDetail([FromBody]RopReplenishGetPlanDetail rop)
        {
            var result = StoreTermServiceFactory.Replenish.GetPlanDetail(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ConfirmReplenish([FromBody]RopReplenishConfirmReplenish rop)
        {
            var result = StoreTermServiceFactory.Replenish.ConfirmReplenish(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
