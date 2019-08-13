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
    public class OwnController : ApiController
    {
        [HttpGet]
        public OwnApiHttpResponse LoginByUrlParams([FromUri]string mId = "", [FromUri]string tppId = "")
        {
            IResult result = InsAppServiceFactory.Own.LoginByUrlParams(mId, tppId);

            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse LoginByAccount(RopOwnLoginByAccount rop)
        {
            IResult result = InsAppServiceFactory.Own.LoginByAccount(rop);

            return new OwnApiHttpResponse(result);
        }
    }
}
