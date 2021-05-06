using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;


namespace WebApiStoreApp.Controllers
{

    public class CartController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse Operate([FromBody]RopCartOperate rop)
        {
            var result = StoreAppServiceFactory.Cart.Operate(this.CurrentUserId, this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);

        }

        [HttpGet]
        public OwnApiHttpResponse<RetCartPageData> PageData([FromUri]RupCartPageData rup)
        {
            var result = StoreAppServiceFactory.Cart.PageData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse<RetCartPageData>(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetCartData([FromUri]RupCartGetCartData rup)
        {
            var result = StoreAppServiceFactory.Cart.GetCartData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }
    }
}