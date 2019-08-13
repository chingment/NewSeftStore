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
    public class UserInfoController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse Save([FromBody]RopUserInfoSave rop)
        {
            IResult result = AccountServiceFactory.UserInfo.Save(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
