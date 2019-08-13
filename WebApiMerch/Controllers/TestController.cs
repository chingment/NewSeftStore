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

            RopProductSkuAdd rop  = new RopProductSkuAdd();
            rop.Name = "dasddd";
            rop.DetailsDes = "Das";
            rop.BarCode = "Dadsad";
            
            MerchServiceFactory.ProductSku.Add("das", "2", rop);
            IResult result = new CustomJsonResult { Result = ResultType.Success, Code = ResultCode.Success, Data = { } };

            return new OwnApiHttpResponse(result);

        }
    }
}