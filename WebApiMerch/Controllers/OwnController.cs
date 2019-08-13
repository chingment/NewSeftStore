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
    public class OwnController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetInfo([FromUri]LocalS.Service.Api.Account.RupOwnGetInfo rup)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.GetInfo(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse CheckPermission([FromUri]LocalS.Service.Api.Account.RupOwnCheckPermission rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.CheckPermission(this.CurrentUserId, this.CurrentUserId,this.Token, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
