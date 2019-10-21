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

        [HttpGet]
        public OwnApiHttpResponse GetSlots([FromUri]RupMachineSlotStocks rup)
        {
            IResult result = StoreTermServiceFactory.Machine.GetSlots(rup.MachineId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveSlot([FromBody]RopMachineSaveSlot rop)
        {
            IResult result = StoreTermServiceFactory.Machine.SaveSlot(rop);
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
        public OwnApiHttpResponse SendRunStatus([FromBody]RopMachineSendRunStatus rop)
        {
            IResult result = StoreTermServiceFactory.Machine.SendRunStatus(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse UpLoadLog()
        {
            LogUtil.Info("进入UpLoadLog");

            var s = this;
            HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];//获取传统context
            HttpRequestBase request = context.Request;//定义传统request对象 

            if (request.Files != null)
            {
                for (int i = 0; i < request.Files.Count; i++)
                {
                    string c = request.Files[i].FileName;

                    LogUtil.Info("file name:" + c);
                }
            }


            if (request.Form.AllKeys != null)
            {
                for (int i = 0; i < request.Form.AllKeys.Length; i++)
                {
                    string key = request.Form.GetKey(i);
                    string value = request.Form[i];
                    LogUtil.Info("file name:" + key + ":" + value);
                }
            }

            var file = request.Files[0];
            string fileExtension = System.IO.Path.GetExtension(file.FileName).ToLower();
            string fileName = GuidUtil.New();
            string filePath = HttpContext.Current.Server.MapPath("/") + ("/log-data-app/");
            string path = filePath + fileName + fileExtension;//获取存储的目标地址
            file.SaveAs(path);

            IResult result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            return new OwnApiHttpResponse(result);
        }
    }
}