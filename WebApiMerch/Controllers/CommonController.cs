using LocalS.Service.Api.Merch;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiMerch.Controllers
{
    public class CommonController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetStores()
        {
            IResult result = MerchServiceFactory.Common.GetStores(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }
    }
}
