using LocalS.Service.Api.StoreTerm;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class ProductController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse SearchSku([FromBody]RopProductSearchSku rup)
        {

            var result = StoreTermServiceFactory.Product.SearchSku(rup);
            return new OwnApiHttpResponse(result);
        }

    }
}
