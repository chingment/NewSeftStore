using Lumos;
using Lumos.Redis;
using MyWeiXinSdk;
using SenvivSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SenvivProvider : BaseService
    {
        public string GetApiAccessToken()
        {
            string name = "qxtadmin";

            string key = string.Format("Senviv_{0}_AccessToken", name);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {

                var loginRequest = new LoginRequest("", new { name = name, pwd = "zkxz123" });

                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

                var result = api.DoPost(loginRequest);

                if (result.Result == ResultType.Success)
                {

                    accessToken = result.Data.Data.AuthorizationCode;

                    redis.KSet(key, accessToken, new TimeSpan(0, 30, 0));

                }

                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));
            }
            else
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}", key, accessToken));
            }

            return accessToken;
        }

        public string GetWxPaAccessToken(string appId)
        {

            string key = string.Format("Wx_AppId_{0}_AccessToken", appId);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));

                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

                var getAccessTokenRequest = new SenvivSdk.GetAccessTokenRequest(GetApiAccessToken(), new { deptid = "32" });


                var result = api.DoPost(getAccessTokenRequest);


                if (result.Result == ResultType.Success)
                {
                    accessToken = result.Data.Data.access_token;

                    LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}，已过期，重新获取成功", key, accessToken));


                    redis.KSet(key, accessToken, new TimeSpan(0, 30, 0));

                }

            }
            else
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，value：{1}", key, accessToken));
            }

            return accessToken;
        }

        public void No(string clientUserId, string productSkuId, string productSkuName, DateTime expireDate, string pOrderId)
        {

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();


            if (string.IsNullOrEmpty(d_ClientUser.WxPaOpenId))
            {
                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

                var userListRequest = new SenvivSdk.UserListRequest(GetApiAccessToken(), new { deptid = "32", size = "100", page = "1", keyword = d_ClientUser.PhoneNumber });

                var result = api.DoPost(userListRequest);

                if (result.Result == ResultType.Success)
                {
                    if (result.Data != null)
                    {
                        if (result.Data.Data != null)
                        {
                            if (result.Data.Data.data != null)
                            {
                                if (result.Data.Data.data.Count > 0)
                                {
                                    var user = result.Data.Data.data[0];

                                    d_ClientUser.WxPaOpenId = user.wechatid;

                                    CurrentDb.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }

            //string openId = d_ClientUser.WxPaOpenId;

            //string first = string.Format("您好，您的{0}租约即使到期", productSkuName);
            //string keyword1 = deviceId;
            //string keyword2 = productSkuName;
            //string keyword3 = string.Format("{0}到期", expireDate.ToString("yyyy年MM月dd日"));
            //string remark = "请尽快充值续费，以免影响您的设备使用！";

            //string mp_AppId = "wx80caad9ea41a00fc";
            //string mp_PagePath = string.Format("pages/orderconfirm/orderconfirm?productSkus=%5B%7B%22cartId%22%3A0%2C%22id%22%3A%22{0}%22%2C%22quantity%22%3A1%2C%22shopMode%22%3A1%2C%22shopMethod%22%3A5%2C%22shopId%22%3A%220%22%7D%5D&shopMethod=5&action=rentfee&pOrderId={1}", productSkuId, pOrderId);
            //StringBuilder sb = new StringBuilder();
            //sb.Append("{\"touser\":\"" + openId + "\",");
            //sb.Append("\"template_id\":\"xCwBMd_h0ekopGsYIj7fpi7-qAY54qbuROTzmS7odhQ\",");
            //sb.Append("\"url\":\"\", ");
            //sb.Append("\"miniprogram\":{");
            //sb.Append("\"appid\":\"" + mp_AppId + "\",");
            //sb.Append("\"pagepath\":\"" + mp_PagePath + "\"");
            //sb.Append("},");
            //sb.Append("\"data\":{");
            //sb.Append("\"first\":{ \"value\":\"" + first + "。\",\"color\":\"#173177\" },");
            //sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            //sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            //sb.Append("\"keyword3\":{ \"value\":\"" + keyword3 + "\",\"color\":\"#FF3030\" },");
            //sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            //sb.Append("}}");


            //WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(GetWxPaAccessToken(d_ClientUser.WxPaAppId), WxPostDataType.Text, sb.ToString());
            //WxApi c = new WxApi();

            //c.DoPost(templateSend);


        }
    }
}
