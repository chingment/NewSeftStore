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
    public class PayTransController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupPayTransGetList rup)
        {
            var result = MerchServiceFactory.PayTrans.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}
