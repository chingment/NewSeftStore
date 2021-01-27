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
    public class IndexController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse<RetIndexPageData> PageData([FromUri]RupIndexPageData rup)
        {
            IResult<RetIndexPageData> result = StoreAppServiceFactory.Index.PageData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse<RetIndexPageData>(result);
        }
        
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse SugProducts([FromUri]RupIndexSugProducts rup)
        {
            IResult result = StoreAppServiceFactory.Index.SugProducts(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }


    }
}
