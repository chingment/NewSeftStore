using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;


namespace WebApiStoreApp.Controllers
{

    public class CouponController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse My([FromBody]RupCouponMy rup)
        {
            IResult result = StoreAppServiceFactory.Coupon.My(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}