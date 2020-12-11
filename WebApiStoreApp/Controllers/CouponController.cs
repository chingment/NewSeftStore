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
    }
}