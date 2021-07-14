using LocalS.BLL;
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
            var result = MerchServiceFactory.Order.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse GetDetails(string id)
        {
            var result = MerchServiceFactory.Order.GetDetails(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDetailsByDeviceSelfTake(string id)
        {
            var result = MerchServiceFactory.Order.GetDetailsByDeviceSelfTake(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse HandleExByDeviceSelfTake(RopOrderHandleExByDeviceSelfTake rop)
        {
            rop.AppId = AppId.MERCH;
            var result = MerchServiceFactory.Order.HandleExByDeviceSelfTake(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SendDeviceShip(RopOrderHandleExByDeviceSelfTake rop)
        {
            rop.AppId = AppId.MERCH;
            var result = MerchServiceFactory.Order.SendDeviceShip(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
