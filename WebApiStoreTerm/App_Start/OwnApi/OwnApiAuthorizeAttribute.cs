using System;
using System.Web;
using System.Collections.Specialized;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Collections.Generic;
using Lumos.Web.Http;
using System.IO;
using System.Text;
using Lumos;
using System.Web.Http;
using System.Linq;
using LocalS.Service.Api.StoreTerm;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Net.Http;

namespace WebApiStoreTerm
{

    public class OwnApiAuthorizeAttribute : ActionFilterAttribute
    {
        public const short MaxTimeDiff = 30;

        private DateTime GetDateTimeTolerance(long timestamp)
        {
            DateTime dt = DateTime.Parse(DateTime.Now.ToString("1970-01-01 00:00:00")).AddSeconds(timestamp);
            var ts = DateTime.Now - dt;
            if (System.Math.Abs(ts.TotalMinutes) > 5)
            {
                dt = DateTime.Now;
            }
            return dt;
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                DateTime requestTime = DateTime.Now;
                var request = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request;
                var httpMethod = request.HttpMethod.ToLower();
                var contentType = request.ContentType.ToLower();
                var rawUrl = request.RawUrl.ToLower();
                string requstBody = null;

                if (contentType.Contains("application/json"))
                {
                    Stream stream = request.InputStream;
                    stream.Seek(0, SeekOrigin.Begin);
                    requstBody = new StreamReader(stream).ReadToEnd();
                }

                var passUrls = new List<string>();

                passUrls.Add("/api/device/upload");

                Log(request, requstBody);

                if (passUrls.Contains(rawUrl))
                {
                    base.OnActionExecuting(actionContext);
                }
                else
                {
                    bool skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
                    if (skipAuthorization)
                    {
                        return;
                    }

                    if (httpMethod != "post")
                    {
                        OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Exception, "只允许POST方式");
                        actionContext.Response = new OwnApiHttpResponse(result);
                        return;
                    }

                    if (!contentType.Contains("application/json"))
                    {
                        OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Exception, "只允许ContentType为application/json");
                        actionContext.Response = new OwnApiHttpResponse(result);
                        return;
                    }

                    string app_id = request.Headers["appId"];
                    string app_key = request.Headers["appKey"];
                    string app_sign = request.Headers["sign"];
                    string app_version = request.Headers["version"];
                    string app_timestamp_s = request.Headers["timestamp"];


                    //LogUtil.Info("Sign_data2:" + requstBody);

                    //检查必要的参数
                    if (app_key == null || app_sign == null || app_timestamp_s == null)
                    {
                        OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Failure2Sign, "缺少必要参数");
                        actionContext.Response = new OwnApiHttpResponse(result);
                        return;
                    }

                    //检查key是否在数据库中存在
                    string app_secret = LocalS.BLL.Biz.BizFactory.AppSoftware.GetAppSecretByAppKey(app_id, app_key);

                    if (app_secret == null)
                    {
                        OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Failure2Sign, "应用程序Key,存在错误");
                        actionContext.Response = new OwnApiHttpResponse(result);
                        return;
                    }

                    long app_timestamp = long.Parse(app_timestamp_s);

                    string signStr = GetSign(app_id, app_key, app_secret, app_timestamp, requstBody);

                    if (IsRequestTimeout(app_timestamp))
                    {
                        OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Failure2Sign, "请求已超时");
                        actionContext.Response = new OwnApiHttpResponse(result);
                        return;
                    }

                    if (signStr != app_sign)
                    {
                        LogUtil.Warn("API签名错误");
                        OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Failure2Sign, "签名错误");
                        actionContext.Response = new OwnApiHttpResponse(result);
                        return;
                    }

                    base.OnActionExecuting(actionContext);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("API错误:{0}", ex.Message), ex);
                OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Exception, ResultCode.Exception, "内部错误");
                actionContext.Response = new OwnApiHttpResponse(result);
                return;
            }

        }

        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            Task.Factory.StartNew(async () =>
            {
                var sb = new StringBuilder();
                string content = await filterContext.Response.Content.ReadAsStringAsync();
                sb.Append("Response: " + content + Environment.NewLine);
                LogUtil.Info(sb.ToString());
            });

        }

        public bool IsRequestTimeout(long app_timestamp)
        {
            System.DateTime time = System.DateTime.MinValue;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime app_requestTime = startTime.AddSeconds(app_timestamp);

            var ts = DateTime.Now - app_requestTime;
            if (System.Math.Abs(ts.TotalMinutes) > MaxTimeDiff)
            {
                return true;
            }

            return false;
        }

        public string GetSign(string app_id, string app_key, string secret, long timespan, string data)
        {
            var sb = new StringBuilder();

            sb.Append(app_id);
            sb.Append(app_key);
            sb.Append(secret);
            sb.Append(timespan.ToString());
            sb.Append(data);

            var material = string.Concat(sb.ToString().OrderBy(c => c));

            var input = Encoding.UTF8.GetBytes(material);

            var hash = SHA256Managed.Create().ComputeHash(input);

            StringBuilder sb2 = new StringBuilder();
            foreach (byte b in hash)
                sb2.Append(b.ToString("x2"));

            string str = sb2.ToString();

            return str;
        }

        private static Task Log(HttpRequestBase request, string requestBody)
        {
            return Task.Factory.StartNew(() =>
            {
                var sb = new StringBuilder();
                sb.Append("Url: " + request.RawUrl + Environment.NewLine);
                sb.Append("IP: " + CommonUtil.GetIpAddress(request) + Environment.NewLine);
                sb.Append("Method: " + request.HttpMethod + Environment.NewLine);
                sb.Append("ContentType: " + request.ContentType + Environment.NewLine);
                NameValueCollection headers = request.Headers;

                if (headers["appKey"] != null)
                {
                    sb.Append("Header.appId: " + headers["appId"] + Environment.NewLine);
                    sb.Append("Header.appKey: " + headers["appKey"] + Environment.NewLine);
                    sb.Append("Header.sign: " + headers["sign"] + Environment.NewLine);
                    sb.Append("Header.version: " + headers["version"] + Environment.NewLine);
                    sb.Append("Header.timestamp: " + headers["timestamp"] + Environment.NewLine);
                }

                if (request.ContentType.Contains("application/json"))
                {
                    sb.Append("PostData: " + requestBody + Environment.NewLine);
                }

                LogUtil.Info(sb.ToString());
            });
        }
    }
}