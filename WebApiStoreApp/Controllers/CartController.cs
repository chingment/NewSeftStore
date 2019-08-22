﻿using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;


namespace WebApiStoreApp.Controllers
{

    public class CartController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse Operate([FromBody]RopCartOperate rop)
        {
            IResult result = StoreAppServiceFactory.Cart.Operate(this.CurrentUserId, this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);

        }

        public OwnApiHttpResponse<RetCartGetPageData> GetPageData([FromUri]RupCartPageData rup)
        {
            IResult<RetCartGetPageData> result = StoreAppServiceFactory.Cart.GetPageData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse<RetCartGetPageData>(result);
        }
    }
}