using System;
using System.Web;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Lumos;
using System.Web.Http;
using Lumos.Web.Http;
using Lumos.Session;

namespace WebApiAccount
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