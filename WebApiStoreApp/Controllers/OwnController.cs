using System;
using System.Web.Http;
using Lumos;
using Lumos.Session;
using LocalS.Service.Api.Account;

namespace WebApiStoreApp.Controllers
{
    public class OwnController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByMinProgram(RopOwnLoginByMinProgram rop)
        {
            IResult result = AccountServiceFactory.Own.LoginByMinProgram(rop);
            return new OwnApiHttpResponse(result);
        }


    }
}