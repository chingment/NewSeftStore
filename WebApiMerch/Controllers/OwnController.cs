﻿using LocalS.Service.Api.Merch;
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
        public OwnApiHttpResponse CheckPermission([FromUri]LocalS.Service.Api.Account.RupOwnCheckPermission rup)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.CheckPermission(this.CurrentUserId, this.CurrentUserId,this.Token, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ChangePassword([FromBody]LocalS.Service.Api.Account.RopOwnChangePassword rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.ChangePassword(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
