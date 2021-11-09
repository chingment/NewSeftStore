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

        public CustomJsonResult NotifyClientExpire(string clientUserId, string skuId, string skuName, DateTime expireDate, string pOrderId)
        {
            LogUtil.Info("NotifyClientExpire");

            var result = new CustomJsonResult();

            var d_ClientUser = CurrentDb.SysClientUser.Where(m => m.Id == clientUserId).FirstOrDefault();

            LogUtil.Info("NotifyClientExpire.PhoneNumber:" + d_ClientUser.PhoneNumber);

            if (string.IsNullOrEmpty(d_ClientUser.PhoneNumber))
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "");


            LogUtil.Info("NotifyClientExpire.pOrderId:" + pOrderId);

            var d_RentOrder = CurrentDb.RentOrder.Where(m => m.OrdeId == pOrderId).FirstOrDefault();

            var d_SenvivUsers = CurrentDb.SenvivUser.Where(m => m.Mobile == d_ClientUser.PhoneNumber).ToList();

            foreach (var d_SenvivUser in d_SenvivUsers)
            {
                LogUtil.Info("NotifyClientExpire.Sn:" + d_RentOrder.SkuDeviceSn);

                var d_SenvivUserProducts = CurrentDb.SenvivUserProduct.Where(m => m.SvUserId == d_SenvivUser.Id && m.Sn == d_RentOrder.SkuDeviceSn).ToList();

                foreach (var d_SenvivUserProduct in d_SenvivUserProducts)
                {
                    LogUtil.Info("NotifyClientExpire.Sn2:" + d_SenvivUserProduct.Sn);
                    LogUtil.Info("NotifyClientExpire.WechatId:" + d_SenvivUser.WechatId);

                    string openId = d_SenvivUser.WechatId;

                    string first = string.Format("您好，您的{0}租约即使到期", skuName);
                    string keyword1 = d_SenvivUserProduct.Sn;
                    string keyword2 = skuName;
                    string keyword3 = string.Format("{0}到期", expireDate.ToString("yyyy年MM月dd日"));
                    string remark = "请尽快续费，以免影响您的设备使用！";

                    string mp_AppId = "wx80caad9ea41a00fc";
                    string mp_PagePath = string.Format("pages/orderconfirm/orderconfirm?skus=%5B%7B%22cartId%22%3A0%2C%22id%22%3A%22{0}%22%2C%22quantity%22%3A1%2C%22shopMode%22%3A1%2C%22shopMethod%22%3A5%2C%22shopId%22%3A%220%22%7D%5D&shopMethod=5&action=rentfee&pOrderId={1}", skuId, pOrderId);
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

                    WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(GetWxPaAccessToken(d_SenvivUserProduct.DeptId), WxPostDataType.Text, sb.ToString());
                    WxApi c = new WxApi();

                    var r = c.DoPost(templateSend);
                }
            }

            result = new CustomJsonResult(ResultType.Success, ResultCode.Success, "");
            return result;
        }

        public List<SenvivSdk.UserListResult.DataModel> GetUserList()
        {
            var list = new List<SenvivSdk.UserListResult.DataModel>();

            SenvivSdk.ApiDoRequest api = new SenvivSdk.ApiDoRequest();

            int page = 1;
            int size = 10;

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


        public bool SendMonthReport(string reportId)
        {
            var report = CurrentDb.SenvivHealthMonthReport.Where(m => m.Id == reportId).FirstOrDefault();

            var user = CurrentDb.SenvivUser.Where(m => m.Id == report.SvUserId).FirstOrDefault();

            var opendId = user.WechatId;
            //var opendId = "on0dM57Q4XOgdHHx-vqmiibI547M";
            //var opendId = "on0dM51JLVry0lnKT4Q8nsJBRXNs";
            var deptId = user.DeptId;
            var template_id = "GpJesR4yR2vO_V9NPgAZ9S2cOR5e3UT3sR58hMa6wKY";
            var healthDate = report.HealthDate;
            var totalScore = report.TotalScore;

            StringBuilder sb = new StringBuilder();
            sb.Append("{\"touser\":\"" + opendId + "\",");
            sb.Append("\"template_id\":\"" + template_id + "\",");
            sb.Append("\"url\":\"http://health.17fanju.com/#/report/month/monitor?rptId=" + reportId + "\", ");
            sb.Append("\"data\":{");
            sb.Append("\"first\":{ \"value\":\"您好，" + healthDate + "月健康报告已生成，详情如下。\",\"color\":\"#173177\" },");
            sb.Append("\"keyword1\":{ \"value\":\"" + DateTime.Now.ToUnifiedFormatDateTime() + "\",\"color\":\"#173177\" },");
            sb.Append("\"keyword2\":{ \"value\":\"总体评分" + totalScore + "分\",\"color\":\"#173177\" },");
            sb.Append("\"remark\":{ \"value\":\"感谢您的支持，如需查看详情报告信息请点击\",\"color\":\"#173177\"}");
            sb.Append("}}");

            WxApiMessageTemplateSend templateSend = new WxApiMessageTemplateSend(GetWxPaAccessToken(deptId), WxPostDataType.Text, sb.ToString());
            WxApi c = new WxApi();

            var ret = c.DoPost(templateSend);

            if (ret.errcode != "0")
                return false;

            return true;
        }

    }
}
