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
    public class Senviv4GProvider : BaseService
    {
        public string GetApiAccessToken()
        {
            string name = "全线通月子会所";

            string key = string.Format("Senviv_{0}_AccessToken", name);

            var redis = new RedisClient<string>();
            var accessToken = redis.KGetString(key);

            if (accessToken == null)
            {

                var loginRequest = new LoginRequest("", new { name = name, pwd = "qxt123456" });

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

        public string GetWxPaAccessToken(string deptId)
        {
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

                    string appId = result.Data.Data.appid;

                    var d_SenvivDept = CurrentDb.SenvivDept.Where(m => m.Id == deptId).FirstOrDefault();

                    if (d_SenvivDept == null)
                    {
                        d_SenvivDept = new Entity.SenvivDept();
                        d_SenvivDept.Id = deptId;
                        d_SenvivDept.AppId = appId;
                        CurrentDb.SenvivDept.Add(d_SenvivDept);
                        CurrentDb.SaveChanges();
                    }

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

        public List<SenvivSdk.UserListResult.DataModel> GetUserList()
        {
            var list = new List<SenvivSdk.UserListResult.DataModel>();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1000;

            var userListRequest = new SenvivSdk.UserListRequest(GetApiAccessToken(), new { deptid = "32", size = size, page = page });
            var result = api.DoPost(userListRequest);
            if (result.Result == ResultType.Success)
            {
                var data = result.Data;
                if (data != null)
                {
                    int count = data.Data.count;
                    int pageCount = (count + size - 1) / size;

                    list.AddRange(data.Data.data);

                    if (pageCount >= 2)
                    {
                        for (var i = 2; i <= pageCount; i++)
                        {
                            var userListRequest2 = new SenvivSdk.UserListRequest(GetApiAccessToken(), new { deptid = "32", size = size, page = i });
                            var result2 = api.DoPost(userListRequest2);
                            if (result2.Result == ResultType.Success)
                            {
                                var data2 = result2.Data;
                                if (data2 != null)
                                {
                                    list.AddRange(data2.Data.data);
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        public List<SenvivSdk.BoxListResult.DataModel> GetBoxList()
        {
            var list = new List<SenvivSdk.BoxListResult.DataModel>();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1000;

            var boxListRequest = new SenvivSdk.BoxListRequest(GetApiAccessToken(), new { deptid = "32", size = size, page = page });
            var result = api.DoPost(boxListRequest);
            if (result.Result == ResultType.Success)
            {
                var data = result.Data;
                if (data != null)
                {
                    int count = data.Data.count;
                    int pageCount = (count + size - 1) / size;

                    list.AddRange(data.Data.data);

                    if (pageCount >= 2)
                    {
                        for (var i = 2; i <= pageCount; i++)
                        {
                            var userListRequest2 = new SenvivSdk.BoxListRequest(GetApiAccessToken(), new { deptid = "32", size = size, page = i });
                            var result2 = api.DoPost(userListRequest2);
                            if (result2.Result == ResultType.Success)
                            {
                                var data2 = result2.Data;
                                if (data2 != null)
                                {
                                    list.AddRange(data2.Data.data);
                                }
                            }
                        }
                    }
                }
            }

            return list;
        }

        public SenvivSdk.BoxListResult.DataModel GetBox(string keyword)
        {
            SenvivSdk.BoxListResult.DataModel model = null;

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 1;

            var boxListRequest = new SenvivSdk.BoxListRequest(GetApiAccessToken(), new { deptid = "32", size = size, page = page, keyword = keyword });
            var result = api.DoPost(boxListRequest);
            if (result.Result == ResultType.Success)
            {
                var data = result.Data;
                if (data != null)
                {
                    model = data.Data.data[0];
                }

            }

            return model;
        }

        public ReportDetailListResult.DataModel GetUserHealthDayReport(string deptid, string userid)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            var requestReportDetailList = new SenvivSdk.ReportDetailListRequest(GetApiAccessToken(), new { deptid = deptid, userid = userid, size = 1, page = 1 });
            var resultReportDetailList = api.DoPost(requestReportDetailList);
            if (resultReportDetailList.Result == ResultType.Success)
            {
                if (resultReportDetailList.Data != null)
                {
                    if (resultReportDetailList.Data.Data != null)
                    {
                        var d = resultReportDetailList.Data.Data.data;
                        if (d != null && d.Count > 0)
                        {
                            return d[0];
                        }
                    }

                }
            }

            return null;
        }

        public bool SendMonthReport(string userId, string first, string keyword1, string keyword2, string remark, string url)
        {
            var template = GetTemplate(userId, "month_report");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"" + url + "\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public bool SendArticle(string userId, string first, string keyword1, string keyword2, string remark, string url)
        {
            var template = GetTemplate(userId, "pregnancy_remind");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"" + url + "\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public bool SendHealthMonitor(string userId, string first, string keyword1, string keyword2, string keyword3, string remark)
        {
            var template = GetTemplate(userId, "health_monitor");

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + template.OpenId + "\",");
            sb.Append("\"template_id\":\"" + template.TemplateId + "\",");
            sb.Append("\"url\":\"\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"" + first + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + keyword1 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"" + keyword2 + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword3\":{ \"value\":\"" + keyword3 + "\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"" + remark + "\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(template.AccessToken, WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

        public WxTemplateModel GetTemplate(string userId, string template)
        {
            var model = new WxTemplateModel();

            var d_User = CurrentDb.SenvivUser.Where(m => m.Id == userId).FirstOrDefault();
            var d_Config = CurrentDb.SenvivMerchConfig.Where(m => m.DeptId == d_User.DeptId).FirstOrDefault();

            model.OpenId = d_User.WxOpenId;
            //model.OpenId = "on0dM51JLVry0lnKT4Q8nsJBRXNs";
            model.SenvivDeptId = d_User.DeptId;
            model.AccessToken = GetWxPaAccessToken(d_User.MerchId);
            switch (template)
            {
                case "month_report":
                    model.TemplateId = "GpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKY";
                    break;
                case "health_monitor":
                    model.TemplateId = "4rfsYerDDF7aVGuETQ3n-Kn84mjIHLBn0H6H8giz7Ac";
                    break;
                case "pregnancy_remind":
                    model.TemplateId = "gB4vyZuiziivwyYm3b1qyooZI2g2okxm4b92tEej7B4";
                    break;
            }

            return model;
        }


        public UserCreateResult UserCreate(object post)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            UserCreateRequest userCreateRequest = new UserCreateRequest(GetApiAccessToken(), post);
            var result = api.DoPost(userCreateRequest);

            return result.Data.Data;
        }


        public BoxBindResult BindBox(string trdUserId, string sn)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            BoxBindRequest request = new BoxBindRequest(GetApiAccessToken(), new { sn = sn, userid = trdUserId, deptid = "46" });
            var result = api.DoPost(request);

            return result.Data.Data;
        }

        public BoxUnBindResult UnBindBox(string trdUserId, string sn)
        {

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();
            BoxUnBindRequest request = new BoxUnBindRequest(GetApiAccessToken(), new { sn = sn, userid = trdUserId, deptid = "46" });
            var result = api.DoPost(request);

            return result.Data.Data;
        }
    }

}
