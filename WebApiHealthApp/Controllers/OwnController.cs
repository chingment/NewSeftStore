using System;
using System.Web.Http;
using Lumos;
using Lumos.Session;
using LocalS.Service.Api.Account;

namespace WebApiHealthApp.Controllers
{
    public class OwnController : OwnApiBaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse LoginByMinProgram(RopOwnLoginByMinProgram rop)
        {
            rop.Ip = CommonUtil.GetIP();

            var result = AccountServiceFactory.Own.LoginByMinProgram(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindPhoneNumberByWx(RopOwnBindPhoneNumberByWx rop)
        {
            var result = AccountServiceFactory.Own.BindPhoneNumberByWx(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse WxApiCode2Session(RopWxApiCode2Session rop)
        {
            var result = AccountServiceFactory.Own.GetWxApiCode2Session(rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse Config(RopGetConfig rop)
        {
            var result = AccountServiceFactory.Own.GetConfig(rop);
            return new OwnApiHttpResponse(result);
        }


        [AllowAnonymous]
        [HttpPost]
        public OwnApiHttpResponse WxPhoneNumber(RopWxGetPhoneNumber rop)
        {
            var result = AccountServiceFactory.Own.GetWxPhoneNumber(rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        public OwnApiHttpResponse GetWxACodeUnlimit(RopOwnGetWxACodeUnlimit rop)
        {
            var result = AccountServiceFactory.Own.GetWxACodeUnlimit(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }


    }
}