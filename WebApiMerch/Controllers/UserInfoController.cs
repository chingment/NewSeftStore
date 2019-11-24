using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
namespace WebApiMerch.Controllers
{
    public class UserInfoController : OwnApiBaseController
    {

        [HttpPost]
        public OwnApiHttpResponse ChangePassword([FromBody]LocalS.Service.Api.Account.RopUserInfoChangePassword rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.UserInfo.ChangePassword(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}