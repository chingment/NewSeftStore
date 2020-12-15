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
            IResult result = StoreAppServiceFactory.Member.GetPromSt(this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }


        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse GetPayLevelSt([FromUri]RupMemberGetPayLevelSt rup)
        {
            IResult result = StoreAppServiceFactory.Member.GetPayLevelSt(this.CurrentUserId, rup);

            return new OwnApiHttpResponse(result);
        }
    }
}
