using LocalS.Service.Api.IotTerm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiIotTerm.Controllers
{
    public class ProductController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse Add(RopProductAdd rop)
        {
            var result = IotTermServiceFactory.Product.Add(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit(RopProductEdit rop)
        {
            var result = IotTermServiceFactory.Product.Edit(this.CurrentMerchId, rop);

            return new OwnApiHttpResponse(result);
        }
    }
}
