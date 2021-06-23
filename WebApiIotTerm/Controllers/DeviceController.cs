using LocalS.Service.Api.IotTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiIotTerm.Controllers
{
    public class DeviceController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse List(RopDeviceList rop)
        {
            var result = IotTermServiceFactory.Device.List(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Stock(RopDeviceStock rop)
        {
            var result = IotTermServiceFactory.Device.Stock(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }
    }
}
