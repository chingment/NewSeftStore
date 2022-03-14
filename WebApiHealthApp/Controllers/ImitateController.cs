using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class ImitateController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse LyingIn()
        {
            var result = HealthAppServiceFactory.Imitate.LyingIn(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }
    }
}
