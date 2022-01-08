using System;
using System.Web;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Lumos;
using System.Web.Http;
using Lumos.Web.Http;
using Lumos.Session;
using System.Collections.Generic;

namespace WebApiHealthApp
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class OwnApiAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                var request = ((HttpContextWrapper)actionContext.Request.Properties["MS_HttpContext"]).Request;
                var requestMethod = request.HttpMethod;
                bool skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();
                if (skipAuthorization)
                {
                    return;
                }

                var token = request.Headers["X-Token"];

                if (string.IsNullOrEmpty(token))
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Failure2Sign, "token不能为空");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    base.OnActionExecuting(actionContext);
                    return;
                }

                var session = new Session();

                var token_val = session.Get<Dictionary<string, string>>(string.Format("token:{0}", token));

                if (token_val == null)
                {
                    OwnApiHttpResult result = new OwnApiHttpResult(ResultType.Failure, ResultCode.Failure2Sign, "token已经超时");
                    actionContext.Response = new OwnApiHttpResponse(result);
                    return;
                }

                LogUtil.Info("userId:" + token_val["userId"]);

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