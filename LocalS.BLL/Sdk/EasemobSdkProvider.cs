using EasemobSdk;
using Lumos;
using Lumos.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class EasemobSdkProvider
    {
        private string GetApiAccessToken()
        {

            string key = "Em_AccessToken";

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {
                LogUtil.Info(string.Format("获取EasemobSdk.AccessToken，key：{0}，已过期，重新获取", key));

                EasemobSdk.ApiDoRequest apiDoRequest = new EasemobSdk.ApiDoRequest();

                TokenRequest tokenRequest = new TokenRequest("client_credentials", "YXA6bQh4SdXsSDq3_RNy3hRoRw", "YXA6gV8I7B64QvVlU3xQrzt6aI2CK5w");

                var doPost = apiDoRequest.DoPost(tokenRequest);

                if (doPost.Result == ResultType.Success)
                {
                    accessToken = doPost.Data.Access_token;

                    redis.KSet(key, accessToken, new TimeSpan(0, 30, 0));

                }
                else
                {
                    LogUtil.Info(string.Format("获取EasemobSdk.AccessToken，key：{0}，已过期，Api重新获取失败", key));
                }
            }
            else
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}", key, accessToken));
            }

            return accessToken;
        }

        public CustomJsonResult<RegisterUserResult> RegisterUser(string userName,string password, string nickname)
        {
            var result = new CustomJsonResult();

            EasemobSdk.ApiDoRequest apiDoRequest = new EasemobSdk.ApiDoRequest();

            RegisterUserRequest registerUserRequest = new RegisterUserRequest(userName, password, nickname);

            apiDoRequest.setAccessToken(GetApiAccessToken());

            var doPost = apiDoRequest.DoPost(registerUserRequest);

            return doPost;
        }
    }
}
