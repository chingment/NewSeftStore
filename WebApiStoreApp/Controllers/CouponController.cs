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
            var result = StoreAppServiceFactory.Coupon.My(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse<RetCouponMy>(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]string id)
        {
            var result = StoreAppServiceFactory.Coupon.Details(this.CurrentUserId, this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse RevPosSt([FromUri]RupCouponRevPosSt rup)
        {
            var result = StoreAppServiceFactory.Coupon.RevPosSt(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Receive([FromBody]RopCouponReceive rop)
        {
            var result = StoreAppServiceFactory.Coupon.Receive(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}