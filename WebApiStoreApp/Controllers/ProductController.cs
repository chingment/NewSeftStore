using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class ProductController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse InitSearchPageData([FromUri]RupProductInitSearchPageData rup)
        {
            var result = StoreAppServiceFactory.Product.InitSearchPageData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);

        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse Search([FromUri]RupProductSearch rup)
        {
            var result = StoreAppServiceFactory.Product.Search(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);

        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]RupProductDetails rup)
        {
            var result = StoreAppServiceFactory.Product.Details(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse SkuStockInfo([FromUri]RupProductDetails rup)
        {
            var result = StoreAppServiceFactory.Product.SkuStockInfo(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

    }
}