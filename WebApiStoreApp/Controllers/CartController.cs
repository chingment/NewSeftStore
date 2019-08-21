using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;


namespace WebAppApi.Controllers
{

    public class CartController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse Operate([FromBody]RopCartOperate rop)
        {
            IResult result = StoreAppServiceFactory.Cart.Operate(this.CurrentUserId, this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);

        }

        public OwnApiHttpResponse GetPageData([FromUri]RupCartPageData rup)
        {
            var data = StoreAppServiceFactory.Cart.GetPageData(this.CurrentUserId, this.CurrentUserId, rup.StoreId);

            return ResponseResult(ResultType.Success, ResultCode.Success, "操作成功", data);
        }
    }
}