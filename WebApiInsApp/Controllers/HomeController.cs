using LocalS.Service.Api.InsApp;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiInsApp.Controllers
{
    public class HomeController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetIndexPageData([FromUri]string mId, [FromUri]string uId)
        {
            IResult result = InsAppServiceFactory.Home.GetIndexPageData(mId, uId);

            return new OwnApiHttpResponse(result);
        }
    }
}