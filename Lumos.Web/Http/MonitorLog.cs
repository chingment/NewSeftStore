using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Lumos.Web.Http
{
    public static class MonitorLog
    {
        public static Task Log(HttpRequestBase request, string requestPayload)
        {
            return Task.Factory.StartNew(() =>
            {
                string _requestUrl = request.RawUrl;
                string _remoteAddress = CommonUtil.GetIpAddress(request);
                string _requestMethod = request.HttpMethod;
                string _requestPayload = null;

                if (request.ContentType.Contains("application/json"))
                {
                    _requestPayload = requestPayload;
                }

                NameValueCollection headers = request.Headers;

                Dictionary<string, string> _requestHeaders = new Dictionary<string, string>();
                for (var i = 0; i < headers.Count; i++)
                {
                    _requestHeaders.Add(headers.Keys[i], string.Join(",", headers.GetValues(i)));
                }

                var ret = new
                {
                    general = new
                    {
                        requestUrl = _requestUrl,
                        requestMethod = _requestMethod,
                        remoteAddress = _remoteAddress,
                    },
                    requestHeaders = _requestHeaders,
                    requestPayload = _requestPayload
                };

                LogUtil.Info(ret.ToJsonString());

            });
        }

        public static Task Log(HttpActionExecutedContext filterContext)
        {
            return Task.Factory.StartNew(async () =>
            {
                var request = ((HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]).Request;


                var response = ((HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]).Response;

                var rawUrl = request.RawUrl.ToLower();

                var responseHeaders = response.Headers;



                Dictionary<string, string> _responseHeaders = new Dictionary<string, string>();
                for (var i = 0; i < responseHeaders.Count; i++)
                {
                    _responseHeaders.Add(responseHeaders.Keys[i], string.Join(",", responseHeaders.GetValues(i)));
                }

                bool isFile = false;
                if (_responseHeaders.ContainsKey("Content-Disposition"))
                {
                    var v = _responseHeaders["Content-Disposition"];
                    if (v != null)
                    {
                        if (v.Contains("attachment"))
                        {
                            isFile = true;
                        }
                    }
                }

                string _responseData = null;
                if (!isFile)
                {
                    _responseData = await filterContext.Response.Content.ReadAsStringAsync();
                }

                var ret = new
                {
                    responseHeaders = _responseHeaders,
                    responseData = _responseData,
                };

                LogUtil.Info(ret.ToJsonString());

            });
        }
    }
}
