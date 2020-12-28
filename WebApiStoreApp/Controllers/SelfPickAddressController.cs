using LocalS.Service.Api.StoreApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class SelfPickAddressController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse List([FromUri]RupSelfPickAddressList rup)
        {
            var result = StoreAppServiceFactory.SelfPickAddress.List(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}
