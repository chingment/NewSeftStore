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
    public class PayRefundController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupPayRefundGetList rup)
        {

            var result = MerchServiceFactory.PayRefund.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse SearchOrder([FromUri]RupPayRefundSearchOrder rup)
        {
            var result = MerchServiceFactory.PayRefund.SearchOrder(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetApplyDetails(string orderId)
        {
            var result = MerchServiceFactory.PayRefund.GetApplyDetails(this.CurrentUserId, this.CurrentMerchId, orderId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Apply([FromBody]RopPayRefundApply rop)
        {
            var result = MerchServiceFactory.PayRefund.Apply(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListByHandle([FromUri]RupPayRefundGetList rup)
        {
            var result = MerchServiceFactory.PayRefund.GetListByHandle(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetHandleDetails(string payRefundId)
        {
            var result = MerchServiceFactory.PayRefund.GetHandleDetails(this.CurrentUserId, this.CurrentMerchId, payRefundId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Handle([FromBody]RopPayRefundHandle rop)
        {
            var result = MerchServiceFactory.PayRefund.Handle(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
