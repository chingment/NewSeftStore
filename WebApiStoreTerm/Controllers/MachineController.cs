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
        public OwnApiHttpResponse UpdateInfo([FromBody]RopMachineUpdateInfo rop)
        {
            IResult result = StoreTermServiceFactory.Machine.UpdateInfo(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Login([FromBody]RopMachineLogin rop)
        {
            IResult result = StoreTermServiceFactory.Machine.Login(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Logout([FromBody]RopMachineLogout rop)
        {
            IResult result = StoreTermServiceFactory.Machine.Logout(this.CurrentUserId, this.Token, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SendRunStatus([FromBody]RopMachineSendRunStatus rop)
        {
            IResult result = StoreTermServiceFactory.Machine.SendRunStatus(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse UpLoadTraceLog(RopAppTraceLog rop)
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
        public OwnApiHttpResponse CheckUpdate(RupMachineCheckUpdate rup)
        {
            IResult result = StoreTermServiceFactory.Machine.CheckUpdate(rup);
            return new OwnApiHttpResponse(result);
        }
        //[HttpPost]
        //[AllowAnonymous]
        //public OwnApiHttpResponse UpLoadLog()
        //{
        //    LogUtil.Info("进入UpLoadLog");

        //    var s = this;
        //    HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
        //    HttpRequestBase request = context.Request;//定义传统request对象 

        //    if (request.Files != null)
        //    {
        //        for (int i = 0; i < request.Files.Count; i++)
        //        {
        //            string c = request.Files[i].FileName;

        //            LogUtil.Info("file name:" + c);
        //        }
        //    }


        //    if (request.Form.AllKeys != null)
        //    {
        //        for (int i = 0; i < request.Form.AllKeys.Length; i++)
        //        {
        //            string key = request.Form.GetKey(i);
        //            string value = request.Form[i];
        //            LogUtil.Info("file name:" + key + ":" + value);
        //        }
        //    }

        //    var file = request.Files[0];
        //    string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
        //    string fileName = GuidUtil.New();
        //    string filePath = HttpContext.Current.Server.MapPath("/") + ("/log-data-app/");
        //    string path = filePath + fileName + fileExtension;//获取存储的目标地址
        //    file.SaveAs(path);

        //    IResult result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
        //    return new OwnApiHttpResponse(result);
        //}
    }
}