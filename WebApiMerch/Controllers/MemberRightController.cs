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
    public class MemberRightController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetLevels()
        {
            IResult result = MerchServiceFactory.MemberRight.GetLevels(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }
    }
}
