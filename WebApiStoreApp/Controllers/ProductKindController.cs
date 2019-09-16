using LocalS.Service.Api.StoreApp;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class ProductKindController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse<RetProductKindPageData> PageData([FromUri]RupProductKindPageData rup)
        {
            IResult<RetProductKindPageData> result = StoreAppServiceFactory.ProductKind.PageData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse<RetProductKindPageData>(result);
        }
    }
}
