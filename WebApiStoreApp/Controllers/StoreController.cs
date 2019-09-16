using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class StoreController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse List([FromUri]RupStoreList rup)
        {
            var result = StoreAppServiceFactory.Store.List(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}
