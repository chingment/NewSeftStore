using System;
using System.Web.Http;
using Lumos;
using Lumos.Session;
using LocalS.Service.Api.Account;
using LocalS.Service.Api.HealthApp;
using MyWeiXinSdk;
using LocalS.BLL;
using Lumos.Redis;
using System.Collections.Generic;
using LocalS.BLL.Biz;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core;
using Aliyun.Acs.Dysmsapi.Model.V20170525;

namespace WebApiHealthApp.Controllers
{
    public class OwnController : OwnApiBaseController
    {
        //我的信息
        [HttpGet]
        public OwnApiHttpResponse Info()
        {
            var result = HealthAppServiceFactory.Own.Info(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse EgyContacts()
        {
            var result = HealthAppServiceFactory.Own.EgyContacts(this.CurrentUserId, this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse AuthUrl(RopOwnAuthUrl rop)
        {
            var result = HealthAppServiceFactory.Own.AuthUrl(rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse AuthInfo(RopOwnAuthInfo rop)
        {
            var result = HealthAppServiceFactory.Own.AuthInfo(rop);
            return new OwnApiHttpResponse(result);
        }

        [AllowAnonymous]
        [HttpGet]
        public OwnApiHttpResponse AuthTokenCheck(string token)
        {
            var result = HealthAppServiceFactory.Own.AuthTokenCheck(token);
            return new OwnApiHttpResponse(result);
        }

  
    }
}