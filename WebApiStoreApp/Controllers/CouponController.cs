using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;


namespace WebApiStoreApp.Controllers
{

    public class CouponController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse<RetCouponMy> My([FromBody]RopCouponMy rop)
        {
            IResult<RetCouponMy> result = StoreAppServiceFactory.Coupon.My(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse<RetCouponMy>(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]string id)
        {
            IResult result = StoreAppServiceFactory.Coupon.Details(this.CurrentUserId, this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse RevCenterSt([FromUri]RupCouponRevCenterSt rup)
        {
            IResult result = StoreAppServiceFactory.Coupon.RevCenterSt(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Receive([FromBody]RopCouponReceive rop)
        {
            IResult result = StoreAppServiceFactory.Coupon.Receive(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}