using LocalS.BLL;
using LocalS.Service.Api.HealthApp;
using Lumos;
using MyWeiXinSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class ConfigController : ApiController
    {
        [HttpPost]
        public OwnApiHttpResponse JsSdk(RopConfigJsSdk rop)
        {
            string accessToken = SdkFactory.Senviv.GetApiAccessToken();
            WxAppInfoConfig config = new WxAppInfoConfig();
            config.AppId = "wxf0d98b28bebd0c82";
            config.AppSecret = "209576db0fa3a24d525b98f9b80676ae";
            config.TrdAccessToken = accessToken;

            var configParams = SdkFactory.Wx.GetJsApiConfigParams(config, rop.RequestUrl);

            var result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", configParams);

            return new OwnApiHttpResponse(result);
        }
    }
}
