using Lumos;
using System.Web.Http;


namespace WebApiInsApp.Controllers
{

    public class TestController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse Test()
        {
            IResult result = new CustomJsonResult { Result = ResultType.Success, Code = ResultCode.Success, Data = { } };

            return new OwnApiHttpResponse(result);

        }
    }
}