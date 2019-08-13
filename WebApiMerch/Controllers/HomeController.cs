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
    public class HomeController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetIndexPageData([FromUri]string mId, [FromUri]string uId)
        {
            IResult result = MerchServiceFactory.Home.GetIndexPageData(this.CurrentUserId, mId, uId);

            return new OwnApiHttpResponse(result);
        }
    }
}