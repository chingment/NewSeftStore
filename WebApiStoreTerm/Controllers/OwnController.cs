using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreTerm.Controllers
{
    public class OwnController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetInfo([FromUri]LocalS.Service.Api.Account.RupOwnGetInfo rup)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.GetInfo(this.CurrentUserId, this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse UploadFingerVeinData([FromBody]LocalS.Service.Api.Account.RopUploadFingerVeinData rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.UploadFingerVeinData(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByFingerVein([FromBody]LocalS.Service.Api.Account.RopOwnLoginByFingerVein rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.LoginByFingerVein(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse LoginByAccount([FromBody]LocalS.Service.Api.Account.RopOwnLoginByAccount rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.LoginByAccount(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Logout([FromBody]LocalS.Service.Api.Account.RopOwnLogout rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.Logout(this.CurrentUserId, this.CurrentUserId, this.Token);
            return new OwnApiHttpResponse(result);
        }

    }
}
