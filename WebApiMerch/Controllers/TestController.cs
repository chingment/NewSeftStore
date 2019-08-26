using LocalS.Service.Api.Merch;
using Lumos;
using System.Web.Http;
using WebApiMerch;

namespace WebApiMerch.Controllers
{

    public class TestController : ApiController
    {
        [HttpGet]
        public OwnApiHttpResponse Test()
        {

            RopPrdProductAdd rop  = new RopPrdProductAdd();
            rop.Name = "dasddd";
            rop.DetailsDes = "Das";
            rop.BarCode = "Dadsad";
            
            MerchServiceFactory.PrdProduct.Add("das", "2", rop);
            IResult result = new CustomJsonResult { Result = ResultType.Success, Code = ResultCode.Success, Data = { } };

            return new OwnApiHttpResponse(result);

        }
    }
}