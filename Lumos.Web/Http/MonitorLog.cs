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
        public static Task Log(HttpRequestBase request, string requestBody)
        {
            return Task.Factory.StartNew(() =>
            {
                string url = request.RawUrl;
                string ip = CommonUtil.GetIpAddress(request);
                string method = request.HttpMethod;
                string contentType = request.ContentType;
                string body = null;

                if (request.ContentType.Contains("application/json"))
                {
                    body = requestBody;
                }

                NameValueCollection headers = request.Headers;

                Dictionary<string, string[]> _headers = new Dictionary<string, string[]>();
                for (var i = 0; i < headers.Count; i++)
                {
                    _headers.Add(headers.Keys[i], headers.GetValues(i));
                }

                var ret = new
                {
                    request = new
                    {
                        url,
                        ip,
                        method,
                        contentType,
                        headers = _headers,
                        body
                    }
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

                //var requestMethod = request.HttpMethod;
                //var contentType = request.ContentType.ToLower();
                var rawUrl = request.RawUrl.ToLower();

                //var headers = response.Headers;



                //Dictionary<string, string[]> _headers = new Dictionary<string, string[]>();
                //for (var i = 0; i < headers.Count; i++)
                //{
                //    _headers.Add(headers.Keys[i], headers.GetValues(i));
                //}


                string content = null;
                if (!rawUrl.Contains("report"))
                {
                    content = await filterContext.Response.Content.ReadAsStringAsync();
                }

                var ret = new
                {
                    response = content,
                };

                LogUtil.Info(ret.ToJsonString());

            });
        }
    }
}
