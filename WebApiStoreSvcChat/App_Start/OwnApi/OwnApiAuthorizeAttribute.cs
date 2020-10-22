using System;
using System.Web;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Lumos;
using System.Web.Http;
using Lumos.Web.Http;
using Lumos.Session;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace WebApiStoreSvcChat
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
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

        public static string GetQueryData(Dictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = sortedParams.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return "";

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = UrlEncode(dem.Current.Value);
                if (!string.IsNullOrEmpty(key))
                {
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
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
                var requestMethod = request.HttpMethod;

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

                if (requestMethod == "POST")
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
                else
                {
                    NameValueCollection queryForm = HttpContext.Current.Request.QueryString;
                    Dictionary<string, string> queryData = new Dictionary<string, string>();
                    for (int f = 0; f < queryForm.Count; f++)
                    {
                        string querykey = queryForm.Keys[f];
                        queryData.Add(querykey, queryForm[querykey]);
                    }
                    app_data = GetQueryData(queryData);
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