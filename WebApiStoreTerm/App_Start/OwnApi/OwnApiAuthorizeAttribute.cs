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
                var passUrls = new List<string>();

                passUrls.Add("/api/device/upload");

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

                    string app_data = null;

                    Stream stream = request.InputStream;
                    stream.Seek(0, SeekOrigin.Begin);
                    app_data = new StreamReader(stream).ReadToEnd();

                    #region 过滤图片
                    if (app_data.LastIndexOf(",\"ImgData\":{") > -1)
                    {
                        //Log.Info("去掉图片之前的数据：" + app_data);
                        int x = app_data.LastIndexOf(",\"ImgData\":{");
                        app_data = app_data.Substring(0, x);
                        app_data += "}";
                        //Log.Info("去掉图片之后的数据：" + app_data);

                    }
                    else if (app_data.LastIndexOf(",\"imgData\":{") > -1)
                    {
                        // Log.Info("去掉图片之前的数据：" + app_data);
                        int x = app_data.LastIndexOf(",\"imgData\":{");
                        app_data = app_data.Substring(0, x);
                        app_data += "}";
                        //Log.Info("去掉图片之后的数据：" + app_data);
                    }

                    #endregion


                    LogUtil.Info("Sign_data:" + app_data);

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

                    string signStr = GetSign(app_id, app_key, app_secret, app_timestamp, app_data);

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
    }
}