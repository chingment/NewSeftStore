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
    public class ServiceFunController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse ScanCodeResult([FromBody]RopServiceFunScanCodeResult rop)
        {
            IResult result = StoreAppServiceFactory.ServiceFun.ScanCodeResult(this.CurrentUserId, this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);
        }
    }
}
