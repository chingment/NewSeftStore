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
        [AllowAnonymous]
        public OwnApiHttpResponse<JsApiConfigParams> JsSdk(RopConfigJsSdk rop)
        {
            //string accessToken = SdkFactory.Senviv.GetApiAccessToken();
            WxAppInfoConfig config = new WxAppInfoConfig();
            config.AppId = "wxc6e80f8c575cf3f5";
            config.AppSecret = "fee895c9923da26a4d42d9c435202b37";
            //config.TrdAccessToken = accessToken;

            var configParams = SdkFactory.Wx.GetJsApiConfigParams(config, rop.RequestUrl);

            return new OwnApiHttpResponse<JsApiConfigParams>(configParams);
        }

  
    }
}
