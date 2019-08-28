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
            IResult result = StoreAppServiceFactory.Cart.Operate(this.CurrentUserId, this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);

        }

        public OwnApiHttpResponse<RetCartPageData> PageData([FromUri]RupCartPageData rup)
        {
            IResult<RetCartPageData> result = StoreAppServiceFactory.Cart.PageData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse<RetCartPageData>(result);
        }
    }
}