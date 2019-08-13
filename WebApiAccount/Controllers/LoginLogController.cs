using LocalS.Service.Api.Account;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAccount.Controllers
{
    public class LoginLogController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupLoginLogGetList rup)
        {
            IResult result = AccountServiceFactory.LoginLog.GetList(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}