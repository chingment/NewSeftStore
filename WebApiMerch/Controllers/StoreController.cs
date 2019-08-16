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
    public class StoreController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupStoreGetList rup)
        {
            IResult result = MerchServiceFactory.Store.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}