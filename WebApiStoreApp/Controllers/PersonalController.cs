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
    public class PersonalController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse<RetPersonalPageData> PageData([FromUri]RupPersonalPageData rup)
        {
            IResult<RetPersonalPageData> result = StoreAppServiceFactory.Personal.PageData(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse<RetPersonalPageData>(result);
        }
    }
}
