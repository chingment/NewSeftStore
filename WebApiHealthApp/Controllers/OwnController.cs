using System;
using System.Web.Http;
using Lumos;
using Lumos.Session;
using LocalS.Service.Api.Account;
using LocalS.Service.Api.HealthApp;
using MyWeiXinSdk;
using LocalS.BLL;
using Lumos.Redis;

namespace WebApiHealthApp.Controllers
{
    public class OwnController : OwnApiBaseController
    {

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse AuthUrl(RopOwnAuthUrl rop)
        {

            var result = new CustomJsonResult();

            var ret = new
            {
                url = string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid=wxc6e80f8c575cf3f5&redirect_uri=" + rop.RedriectUrl + "&response_type=code&scope=snsapi_userinfo&state=STATE#wechat_redirect")
            };

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public OwnApiHttpResponse AuthInfo(RopOwnAuthInfo rop)
        {

            var result = new CustomJsonResult();

            WxAppInfoConfig config = new WxAppInfoConfig();
            config.AppId = "wxc6e80f8c575cf3f5";
            config.AppSecret = "fee895c9923da26a4d42d9c435202b37";

            var oauth2_Result = SdkFactory.Wx.GetWebOauth2AccessToken(config, rop.Code);

            string token = null;
            if (oauth2_Result.errcode == null)
            {
                var userInfo_Result = SdkFactory.Wx.GetUserInfo(oauth2_Result.access_token, oauth2_Result.openid);
                if (userInfo_Result != null)
                {
                    var authInfo_Result = HealthAppServiceFactory.Own.AuthInfo("", "88273829", userInfo_Result.openid, userInfo_Result.nickname, userInfo_Result.headimgurl);
                    if (authInfo_Result.Result == ResultType.Success)
                    {
                        var data = authInfo_Result.Data;
                        var token_val = new { userId = data.Id };

                        var session = new Session();
                        token = string.Format("token:{0}", IdWorker.Build(IdType.NewGuid));
                        session.Set<Object>(token, token_val, new TimeSpan(24, 0, 0));
                    }
                }
            }

            if (string.IsNullOrEmpty(token))
            {
                result = new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");
            }
            else
            {
                var ret = new { token = token };
                result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "", ret);
            }

            return new OwnApiHttpResponse(result);
        }


    }
}