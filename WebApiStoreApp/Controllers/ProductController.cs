using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class ProductController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse List([FromUri]RupProductList rup)
        {
            var result = StoreAppServiceFactory.ProductSku.List(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);

        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]RupProductDetails rup)
        {
            var result = StoreAppServiceFactory.ProductSku.Details(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

    }
}