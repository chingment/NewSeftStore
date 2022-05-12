using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class InviteController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse InitRpFollow([FromUri]RopInviteInitRpFollow rop)
        {
            var result = HealthAppServiceFactory.Invite.InitRpFollow(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse AgreeRpFollow(RopInviteAgreeRpFollow rop)
        {
            var result = HealthAppServiceFactory.Invite.AgreeRpFollow(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
