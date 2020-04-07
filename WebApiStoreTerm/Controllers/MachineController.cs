using LocalS.Service.Api.StoreTerm;
using Lumos;
using System.IO;
using System.Web;
using System.Web.Http;


namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class MachineController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse InitData([FromBody]RopMachineInitData rop)
        {
            IResult result = StoreTermServiceFactory.Machine.InitData(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse UpLoadTraceLog([FromBody]RopAppTraceLog rop)
        {
            var request = ((HttpContextWrapper)Request.Properties["MS_HttpContext"]).Request;

            if (request.Headers["data_head"] != null)
            {
                string data_head = System.Web.HttpUtility.UrlDecode(request.Headers["data_head"].ToString());
                LogUtil.Info("data_head:" + data_head);

                rop.device = Newtonsoft.Json.JsonConvert.DeserializeObject<RopAppTraceLog.Device>(data_head);
            }
            else
            {
                LogUtil.Info("data_head: NULL");
            }

            StoreTermServiceFactory.Machine.UpLoadTraceLog(rop);

            IResult result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public OwnApiHttpResponse CheckUpdate([FromUri]RupMachineCheckUpdate rup)
        {
            IResult result = StoreTermServiceFactory.Machine.CheckUpdate(rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse EventNotify([FromBody]RopMachineEventNotify rop)
        {
            IResult result = StoreTermServiceFactory.Machine.EventNotify(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }


        public OwnApiHttpResponse GetRunExHandleItems([FromUri]RupMachineGetRunExHandleItems rup)
        {
            IResult result = StoreTermServiceFactory.Machine.GetRunExHandleItems(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}