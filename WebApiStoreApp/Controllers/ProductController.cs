using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class ProductController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse List([FromUri]RupProductList rup)
        {
            var result = StoreAppServiceFactory.Product.List(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);

        }


        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]RupProductDetails rup)
        {
            var result = StoreAppServiceFactory.Product.Details(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

    }
}