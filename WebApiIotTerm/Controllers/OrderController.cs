using LocalS.Service.Api.IotTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiIotTerm.Controllers
{
    public class OrderController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse Reserve(RopOrderReserve rop)
        {
            var result = IotTermServiceFactory.Order.Reserve(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Cancle(RopOrderCancle rop)
        {
            var result = IotTermServiceFactory.Order.Cancle(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Query(RopOrderQuery rop)
        {
            var result = IotTermServiceFactory.Order.Query(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }
    }
}
