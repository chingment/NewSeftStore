using LocalS.Service.Api.StoreSvcChat;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreSvcChat.Controllers
{
    public class OwnController : OwnApiBaseController
    {

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByAccount([FromBody]LocalS.Service.Api.Account.RopOwnLoginByAccount rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.LoginByAccount(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Logout([FromBody]LocalS.Service.Api.Account.RopOwnLogout rop)
        {
            if (rop == null)
            {
                rop = new LocalS.Service.Api.Account.RopOwnLogout();
            }

            rop.Token = this.Token;


            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.Logout(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse GetContactInfos([FromBody]RopOwnGetContactInfos rop)
        {
            IResult result = StoreSvcChatServiceFactory.Own.GetContactInfos(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
