using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class DeviceController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse InitBind()
        {
            var result = HealthAppServiceFactory.Device.InitBind(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitInfo()
        {
            var result = HealthAppServiceFactory.Device.InitInfo(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }
    }
}
