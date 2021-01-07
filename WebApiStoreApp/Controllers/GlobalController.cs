using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;


namespace WebApiStoreApp.Controllers
{

    public class GlobalController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse DataSet([FromUri]RupGlobalDataSet rup)
        {
            IResult result = StoreAppServiceFactory.Global.DataSet(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse MsgTips([FromUri]RupGlobalMsgTips rup)
        {
            IResult result = StoreAppServiceFactory.Global.MsgTips(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse ByPoint([FromBody]LocalS.BLL.Biz.RopByPoint rop)
        {
            IResult result = LocalS.BLL.Biz.BizFactory.ByPoint.Record(this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);
        }

    }
}