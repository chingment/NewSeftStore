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
        [HttpGet]
        public OwnApiHttpResponse SearchSku([FromUri]RupProductSearchSku rup)
        {

            var result = StoreTermServiceFactory.Product.SearchSku(rup);
            return new OwnApiHttpResponse(result);
        }

    }
}
