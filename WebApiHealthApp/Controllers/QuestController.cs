using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class QuestController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse InitFill()
        {
            var result = HealthAppServiceFactory.Quest.InitFill(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }
    }
}
