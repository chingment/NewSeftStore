using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class EgyContactController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetDetails([FromUri]string id = "")
        {
            var result = HealthAppServiceFactory.EgyContact.GetDetails(this.CurrentUserId, this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Save(RopEgyContactSave rop)
        {
            var result = HealthAppServiceFactory.EgyContact.Save(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
