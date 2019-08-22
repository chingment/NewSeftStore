using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class ProductSkuController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse List([FromUri]RupProductSkuList rup)
        {
            var result = StoreAppServiceFactory.ProductSku.List(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);

        }


        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]RupProductSkuDetails rup)
        {
            var result = StoreAppServiceFactory.ProductSku.Details(rup.SkuId);

            return new OwnApiHttpResponse(result);
        }

    }
}