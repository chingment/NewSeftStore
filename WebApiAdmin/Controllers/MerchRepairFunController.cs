using LocalS.Service.Api.Admin;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAdmin.Controllers
{
    public class MerchRepairFunController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse ReLoadProductSkuCache()
        {
            IResult result = AdminServiceFactory.MerchRepairFun.ReLoadProductCache(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }
    }
}
