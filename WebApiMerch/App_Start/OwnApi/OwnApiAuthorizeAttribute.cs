using System;
using System.Web;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Lumos;
using System.Web.Http;
using Lumos.Web.Http;
using Lumos.Session;
using LocalS.Service.Api.Merch;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace WebApiMerch
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class OwnApiAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                DateTime requestTime = DateTime.Now;
                var request = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request;
                var requestMethod = request.HttpMethod;
                var contentType = request.ContentType.ToLower();
                string requstBody = null;

                if (contentType.Contains("application/json"))
                {
                    Stream stream = request.InputStream;
                    stream.Seek(0, SeekOrigin.Begin);
                    requstBody = new StreamReader(stream).ReadToEnd();
                }

                Log(request, requstBody);


                bool skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
                if (skipAuthorization)
                {
                    return;
                }

                var token = request.QueryString["token"];
                if (token == null)
                {
                    token = request.Headers["X-Token"];
                }

                if (string.IsNullOrEmpty(token))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.NeedLogin, "token不能为空");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    base.OnActionExecuting(actionContext);
                    return;
                }

                var tokenInfo = SSOUtil.GetTokenInfo(token);

                if (tokenInfo == null)
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.NeedLogin, "token 已经超时");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }


                if (string.IsNullOrEmpty(tokenInfo.BelongId))
                {
                    var merchUser = MerchServiceFactory.AdminUser.GetInfo(tokenInfo.UserId);
                    if (merchUser == null)
                    {
                        OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.NeedLogin, "没有权限访问该系统");
                        actionContext.Response = new OwnApiHttpResponse(result);
                        return;
                    }

                    tokenInfo.BelongId = merchUser.MerchId;
                    tokenInfo.BelongType = Lumos.DbRelay.Enumeration.BelongType.Merch;

                    SSOUtil.SetTokenInfo(token, tokenInfo, new TimeSpan(1, 0, 0));

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


        public override void OnActionExecuted(HttpActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            Task.Factory.StartNew(async () =>
            {
                var request = ((HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]).Request;
                var response = ((HttpContextWrapper)filterContext.Request.Properties["MS_HttpContext"]).Response;
                var requestMethod = request.HttpMethod;
                var contentType = request.ContentType.ToLower();
                var rawUrl = request.RawUrl.ToLower();

                var headers = response.Headers;


                //string[] contentDisposition = null;
                //if (headers.["content-Disposition"] != null)
                //{
                //    contentDisposition = headers.GetValues("content-Disposition");
                //}

                Dictionary<string, string[]> _headers = new Dictionary<string, string[]>();
                for (var i = 0; i < headers.Count; i++)
                {
                    _headers.Add(headers.Keys[i], headers.GetValues(i));
                }


                string content = null;
                if (!rawUrl.Contains("report"))
                {
                    content = await filterContext.Response.Content.ReadAsStringAsync();
                }

                var ret = new
                {
                    headers = _headers,
                    response = content,
                };

                LogUtil.Info(ret.ToJsonString());

            });

        }


        private static Task Log(HttpRequestBase request, string requestBody)
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
    }
}