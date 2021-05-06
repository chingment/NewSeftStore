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
            var result = StoreAppServiceFactory.ServiceFun.ScanCodeResult(this.CurrentUserId, this.CurrentUserId, rop);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMyReffSkus([FromUri]RupServiceFunGetMyReffSkus rup)
        {
            var result = StoreAppServiceFactory.ServiceFun.GetMyReffSkus(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

    }
}
