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
    public class UserController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetInfoByUserName([FromUri]RupUserGetInfo rup)
        {
            var result = StoreSvcChatServiceFactory.User.GetInfoByUserName(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}
