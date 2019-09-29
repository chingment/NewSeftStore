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
            IResult result = StoreTermServiceFactory.Order.Reserve(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse<RetOrderPayStatusQuery> PayStatusQuery([FromUri]RupOrderPayStatusQuery rup)
        {
            IResult<RetOrderPayStatusQuery> result = StoreTermServiceFactory.Order.PayStatusQuery(rup);
            return new OwnApiHttpResponse<RetOrderPayStatusQuery>(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Cancle([FromBody]RopOrderCancle rop)
        {
            IResult result = StoreTermServiceFactory.Order.Cancle(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]RupOrderDetails rup)
        {
            IResult result = StoreTermServiceFactory.Order.Details(rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse PickupStatusQuery([FromUri]RupOrderPickupStatusQuery rup)
        {
            IResult result = StoreTermServiceFactory.Order.PickupStatusQuery(rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse PickupEventNotify([FromBody]RopOrderPickupEventNotify rop)
        {
            IResult result = StoreTermServiceFactory.Order.PickupEventNotify(rop);
            return new OwnApiHttpResponse(result);
        }
    }
}