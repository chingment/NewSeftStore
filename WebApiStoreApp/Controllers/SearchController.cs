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
    public class SearchController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse TobeSearch([FromUri]RupSearchTobeSearch rup)
        {
            IResult result = StoreAppServiceFactory.Search.TobeSearch(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }
    }
}
