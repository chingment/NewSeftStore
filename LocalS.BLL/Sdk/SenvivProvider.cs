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

        public string GetWxPaAccessToken(string deptId, out string appId)
        {
            appId = "";

            string key = string.Format("Wx_AppId_{0}_AccessToken", deptId);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {
                LogUtil.Info(string.Format("获取微信AccessToken，key：{0}，已过期，重新获取", key));

                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

                var getAccessTokenRequest = new SenvivSdk.GetAccessTokenRequest(GetApiAccessToken(), new { deptid = deptId });

                var result = api.DoPost(getAccessTokenRequest);

                if (result.Result == ResultType.Success)
                {
                    accessToken = result.Data.Data.access_token;
                    appId = result.Data.Data.appid;

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

        public CustomJsonResult NotifyClientExpire(string clientUserId, string skuId, string skuName, DateTime expireDate, string pOrderId)
        {
            var result = new CustomJsonResult();

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            var deptId = "32";
            var appId = "";

            List<string> devices = new List<string>();

            if (string.IsNullOrEmpty(d_ClientUser.WxPaOpenId))
            {
                GetWxPaAccessToken(deptId, out appId);

                SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
                var userListRequest = new SenvivSdk.UserListRequest(GetApiAccessToken(), new { deptid = deptId, size = "100", page = "1", keyword = d_ClientUser.PhoneNumber });

                var api_Result = api.DoPost(userListRequest);

                if (api_Result.Result == ResultType.Success)
                {
                    if (api_Result.Data != null)
                    {
                        if (api_Result.Data.Data != null)
                        {
                            if (api_Result.Data.Data.data != null)
                            {
                                if (api_Result.Data.Data.data.Count > 0)
                                {
                                    var user = api_Result.Data.Data.data[0];

                                    d_ClientUser.WxPaOpenId = user.wechatid;
                                    d_ClientUser.WxPaAppId = appId;
                                    CurrentDb.SaveChanges();

                                    if (user.products != null)
                                    {
                                        foreach (var products in user.products)
                                        {
                                            devices.Add(products.sn);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            foreach (var device in devices)
            {
                string openId = d_ClientUser.WxPaOpenId;

                string first = string.Format("您好，您的{0}租约即使到期", skuName);
                string keyword1 = device;
                string keyword2 = skuName;
                string keyword3 = string.Format("{0}到期", expireDate.ToString("yyyy年MM月dd日"));
                string remark = "请尽快充值续费，以免影响您的设备使用！";

                string mp_AppId = "wx80caad9ea41a00fc";
                string mp_PagePath = string.Format("pages/orderconfirm/orderconfirm?productSkus=%5B%7B%22cartId%22%3A0%2C%22id%22%3A%22{0}%22%2C%22quantity%22%3A1%2C%22shopMode%22%3A1%2C%22shopMethod%22%3A5%2C%22shopId%22%3A%220%22%7D%5D&shopMethod=5&action=rentfee&pOrderId={1}", skuId, pOrderId);
                StringBuilder sb = new StringBuilder();
                sb.Append("{\"touser\":\"" + openId + "\",");
                sb.Append("\"template_id\":\"xCwBMd_h0ekopGsYIj7fpi7-qAY54qbuROTzmS7odhQ\",");
                sb.Append("\"url\":\"\", ");
                sb.Append("\"miniprogram\":{");
                sb.Append("\"appid\":\"" + mp_AppId + "\",");
                sb.Append("\"pagepath\":\"" + mp_PagePath + "\"");
                sb.Append("},");
                sb.Append("\"data\":{");
                sb.Append("\"first\":{ \"value\":\"" + first + "。\",\"color\":\"#173177\" },");
                sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
                sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
                sb.Append("\"keyword3\":{ \"value\":\"" + keyword3 + "\",\"color\":\"#FF3030\" },");
                sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
                sb.Append("}}");


                WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(GetWxPaAccessToken(deptId, out appId), WxPostDataType.Text, sb.ToString());
                WxApi c = new WxApi();

                var r = c.DoPost(templateSend);
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            return result;
        }

    }
}
