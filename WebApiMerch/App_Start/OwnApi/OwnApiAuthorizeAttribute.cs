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
                var requestMethod = request.HttpMethod;
                var contentType = request.ContentType.ToLower();
                var rawUrl = request.RawUrl.ToLower();
                if (!rawUrl.Contains("report"))
                {
                    var sb = new StringBuilder();
                    string content = await filterContext.Response.Content.ReadAsStringAsync();
                    sb.Append("Response: " + content + Environment.NewLine);
                    LogUtil.Info(sb.ToString());
                }
            });

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