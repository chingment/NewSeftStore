using LocalS.Service.Api.StoreApp;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class MemberController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse GetPromSt([FromUri]RupMemberGetPromSt rup)
        {
            var result = StoreAppServiceFactory.Member.GetPromSt(this.CurrentUserId,this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }


        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse GetPayLevelSt([FromUri]RupMemberGetPayLevelSt rup)
        {
            var result = StoreAppServiceFactory.Member.GetPayLevelSt(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse GetRightDescSt([FromUri]RupMemberGetRightDescSt rup)
        {
            var result = StoreAppServiceFactory.Member.GetRightDescSt(this.CurrentUserId, this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }

    }
}
