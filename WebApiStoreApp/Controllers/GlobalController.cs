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
            var result = StoreAppServiceFactory.Global.DataSet(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse MsgTips([FromUri]RupGlobalMsgTips rup)
        {
            var result = StoreAppServiceFactory.Global.MsgTips(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse ByPoint([FromBody]LocalS.BLL.Biz.RopByPoint rop)
        {
            var result = LocalS.BLL.Biz.BizFactory.ByPoint.Record(this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse GetWxSceneData([FromUri]string scene)
        {
            var result = StoreAppServiceFactory.Global.GetWxSceneData(this.CurrentUserId, scene);

            return new OwnApiHttpResponse(result);
        }

    }
}