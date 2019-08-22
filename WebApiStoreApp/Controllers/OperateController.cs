using System.Web.Http;
using Lumos;
using Lumos.BLL;
using LocalS.Service.Api.StoreApp;

namespace WebApiStoreApp.Controllers
{
    public class OperateController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetResult([FromUri]RupOperateGetResult rup)
        {
            IResult result = StoreAppServiceFactory.Operate.GetResult(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }
    }
}