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
    public class SupplierController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse Search([FromUri]string key)
        {
            var result = MerchServiceFactory.Supplier.Search(this.CurrentUserId, this.CurrentMerchId, key);
            return new OwnApiHttpResponse(result);
        }

    }
}
