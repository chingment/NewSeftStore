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
    public class SmCfSelfTakeOrderController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse CfTake([FromBody]RopSmCfSelfTakeOrderCfTake rop)
        {
            IResult result = StoreAppServiceFactory.SmCfSelfTakeOrder.CfTake(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Details(string id)
        {
            IResult result = StoreAppServiceFactory.SmCfSelfTakeOrder.Details(this.CurrentUserId, this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }
    }
}
