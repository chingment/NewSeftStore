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
    public class OrderController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupOrderGetList rup)
        {
            IResult result = MerchServiceFactory.Order.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDetails(string id)
        {
            IResult result = MerchServiceFactory.Order.GetDetails(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse PickupExceptionHandle([FromBody]RopOrderPickupExceptionHandle rop)
        {
            IResult result = MerchServiceFactory.Order.PickupExceptionHandle(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
