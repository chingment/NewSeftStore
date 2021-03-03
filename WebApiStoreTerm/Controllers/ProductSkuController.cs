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
    public class ProductSkuController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse Search([FromUri]RupSkuSearch rup)
        {

            IResult result = StoreTermServiceFactory.ProductSku.Search(rup);
            return new OwnApiHttpResponse(result);
        }

    }
}
