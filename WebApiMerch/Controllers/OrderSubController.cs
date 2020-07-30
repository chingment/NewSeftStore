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
    public class OrderSubController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupOrderSubGetList rup)
        {
            IResult result = MerchServiceFactory.OrderSub.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse GetListByDelivery([FromUri]RupOrderSubGetList rup)
        {
            IResult result = MerchServiceFactory.OrderSub.GetListByDelivery(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListByStoreSelfTake([FromUri]RupOrderSubGetList rup)
        {
            IResult result = MerchServiceFactory.OrderSub.GetListByStoreSelfTake(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListByMachineSelfTake([FromUri]RupOrderSubGetList rup)
        {
            IResult result = MerchServiceFactory.OrderSub.GetListByMachineSelfTake(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDetailsByMachineSelfTake(string id)
        {
            IResult result = MerchServiceFactory.OrderSub.GetDetailsByMachineSelfTake(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        public OwnApiHttpResponse HandleExByMachineSelfTake(RopOrderSubHandleExByMachineSelfTake rop)
        {
            IResult result = MerchServiceFactory.OrderSub.HandleExByMachineSelfTake(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
