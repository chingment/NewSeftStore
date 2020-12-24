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

        [HttpPost]
        public OwnApiHttpResponse BindPhoneNumberByWx(RopOwnBindPhoneNumberByWx rop)
        {
            IResult result = AccountServiceFactory.Own.BindPhoneNumberByWx(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse WxApiCode2Session(RopWxApiCode2Session rop)
        {
            IResult result = AccountServiceFactory.Own.GetWxApiCode2Session(rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse Config(RopGetConfig rop)
        {
            IResult result = AccountServiceFactory.Own.GetConfig(rop);
            return new OwnApiHttpResponse(result);
        }


        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse WxPhoneNumber(RopWxGetPhoneNumber rop)
        {
            IResult result = AccountServiceFactory.Own.GetWxPhoneNumber(rop);
            return new OwnApiHttpResponse(result);
        }
    }
}