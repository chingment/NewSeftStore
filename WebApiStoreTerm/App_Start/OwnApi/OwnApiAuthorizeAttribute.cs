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

namespace WebApiStoreTerm
{

    public class OwnApiAuthorizeAttribute : ActionFilterAttribute
    {
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

        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if (HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    sb.Append(HttpUtility.UrlEncode(c.ToString()).ToUpper());
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        public static string GetQueryData(NameValueCollection names)
        {
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (names == null || names.Count == 0)
                return "";

            for (int f = 0; f < names.Count; f++)
            {
                queryStr.Append("&").Append(names.Keys[f]).Append("=").Append(names[f]);
            }

            string s = queryStr.ToString().Substring(1, queryStr.Length - 1);

            return s;
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


                string app_id = request.Headers["appId"];
                string app_key = request.Headers["appKey"];
                string app_sign = request.Headers["sign"];
                string app_version = request.Headers["version"];
                string app_timestamp_s = request.Headers["timestamp"];

                string app_data = null;

                if (httpMethod == "POST")
                {
                    if (contentType.Contains("application/json"))
                    {
                        Stream stream = HttpContext.Current.Request.InputStream;
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
                    }
                    else if (contentType.Contains("multipart/form-data"))
                    {
                        NameValueCollection queryForm = request.Form;
                        app_data = GetQueryData(queryForm);
                    }

                    LogUtil.Info("Sign_data:" + app_data);
                }
                else if (httpMethod == "GET")
                {
                    NameValueCollection queryForm = request.QueryString;
                    app_data = GetQueryData(queryForm);
                }

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

                string signStr = Signature.Compute(app_id, app_key, app_secret, app_timestamp, app_data);

                if (Signature.IsRequestTimeout(app_timestamp))
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
            catch (Exception ex)
            {
                LogUtil.Error(string.Format("API错误:{0}", ex.Message), ex);
                OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Exception, ResultCode.Exception, "内部错误");
                actionContext.Response = new OwnApiHttpResponse(result);
                return;
            }

        }
    }
}