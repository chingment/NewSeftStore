using System.Web.Http;
using Lumos;
using Lumos.BLL;
using LocalS.Service.Api.StoreApp;

namespace WebApiStoreApp.Controllers
{
    public class OperateController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse Result([FromUri]RupOperateResult rup)
        {
            IResult result = StoreAppServiceFactory.Operate.Result(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }
    }
}