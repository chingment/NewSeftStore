﻿using Lumos;
using System.Web.Http.Filters;

namespace WebApiIotTerm
{
    public class OwnApiExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            LogUtil.Error("API调用出现异常", actionExecutedContext.Exception);
            OwnApiHttpResult result = new OwnApiHttpResult(ResultCode.Exception, "程序发生异常");
            actionExecutedContext.Response = new OwnApiHttpResponse(result);
            base.OnException(actionExecutedContext);
        }
    }

}