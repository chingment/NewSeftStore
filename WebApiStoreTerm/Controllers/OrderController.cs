using LocalS.Service.Api.StoreTerm;
using Lumos;
using System.Web;
using System.Web.Http;

namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class OrderController : OwnApiBaseController
    {

        [HttpPost]
        public OwnApiHttpResponse Reserve([FromBody]RopOrderReserve rop)
        {
            var result = StoreTermServiceFactory.Order.Reserve(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse<RetOrderPayStatusQuery> PayStatusQuery([FromBody]RopOrderPayStatusQuery rup)
        {
            var result = StoreTermServiceFactory.Order.PayStatusQuery(rup);
            return new OwnApiHttpResponse<RetOrderPayStatusQuery>(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Cancle([FromBody]RopOrderCancle rop)
        {
            var result = StoreTermServiceFactory.Order.Cancle(rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        public OwnApiHttpResponse BuildPayParams([FromBody]RopOrderBuildPayParams rop)
        {
            var result = StoreTermServiceFactory.Order.BuildPayParams(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SearchByPickupCode([FromBody]RopOrderSearchByPickupCode rop)
        {
            var result = StoreTermServiceFactory.Order.SearchByPickupCode(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse PickupStatusQuery([FromBody]RopOrderPickupStatusQuery rop)
        {
            var result = StoreTermServiceFactory.Order.PickupStatusQuery(rop);
            return new OwnApiHttpResponse(result);
        }
    }
}