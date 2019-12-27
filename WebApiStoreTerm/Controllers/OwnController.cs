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

        [HttpPost]
        public OwnApiHttpResponse UploadFingerVeinData([FromUri]LocalS.Service.Api.Account.RopUploadFingerVeinData rop)
        {
            IResult result = LocalS.Service.Api.Account.AccountServiceFactory.Own.UploadFingerVeinData(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
