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

namespace WebApiIotTerm
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

        public string[] GetAuthorization(string authorization)
        {
            string[] arr_auth = new string[3];

            string[] arr_auth2 = authorization.Split(',');

            foreach (var item in arr_auth2)
            {
                string[] keyvalue = item.Split('=');

                if (keyvalue[0] == "merch_id")
                {
                    arr_auth[0] = keyvalue[1];
                }
                else if (keyvalue[0] == "timestamp")
                {
                    arr_auth[1] = keyvalue[1];
                }
                else if (keyvalue[0] == "sign")
                {
                    arr_auth[2] = keyvalue[1];
                }
            }

            return arr_auth;

        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                DateTime requestTime = DateTime.Now;
                var request = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request;
                var httpMethod = request.HttpMethod;
                var contentType = request.ContentType;

                bool skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
                if (skipAuthorization)
                {
                    return;
                }

                if (httpMethod.ToUpper() != "POST")
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "只允许POST方式");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                if (!contentType.Contains("application/json"))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "只允许ContentType为application/json");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                if (request.Headers["Authorization"] == null)
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "Header.Authorization 不能为空");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                string str_auth = request.Headers["Authorization"];

                string[] arr_auth = GetAuthorization(str_auth);

                if (string.IsNullOrEmpty(arr_auth[0]))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "Header.Authorization,merch_id 不能为空");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                if (string.IsNullOrEmpty(arr_auth[1]))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "Header.Authorization,timestamp 不能为空");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                if (string.IsNullOrEmpty(arr_auth[1]))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "Header.Authorization,sign 不能为空");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }


                Stream stream = request.InputStream;
                stream.Seek(0, SeekOrigin.Begin);

                string post_data = new StreamReader(stream).ReadToEnd();


                //检查key是否在数据库中存在
                string api_secret = LocalS.BLL.Biz.BizFactory.Merch.GetIotApiSecret(arr_auth[0]);
                if (string.IsNullOrEmpty(api_secret))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Failure2Sign, "应用程序Key,存在错误");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                long timestamp = long.Parse(arr_auth[1]);

                if (IsRequestTimeout(timestamp))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Failure2Sign, "请求已超时");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }


                string signStr = GetSign(arr_auth[0], api_secret, timestamp, post_data);


                if (signStr != arr_auth[2])
                {
                    LogUtil.Warn("API签名错误");
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Failure2Sign, "签名错误");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                base.OnActionExecuting(actionContext);
            }
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("API错误:{0}", ex.Message), ex);
                OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "内部错误");
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

        public string GetSign(string merch_id, string secret, long timespan, string data)
        {
            var sb = new StringBuilder();

            sb.Append(merch_id);
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