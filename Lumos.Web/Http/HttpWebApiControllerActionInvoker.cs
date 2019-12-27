﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Lumos.Web.Http
{
    public class HttpWebApiControllerActionInvoker : ApiControllerActionInvoker
    {
        public override Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var responseMessage = base.InvokeActionAsync(actionContext, cancellationToken);

            if (responseMessage.Exception != null)
            {
                var baseException = responseMessage.Exception.InnerExceptions[0];

                var result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "系统错误");

                if (baseException is TimeoutException)
                {
                    result.Result = ResultType.Exception;
                    result.Code = ResultCode.Exception;
                    result.Message = "请求超时";
                }

                var t = new HttpResponseMessage { Content = new StringContent(result.ToString(), Encoding.GetEncoding("UTF-8"), "application/json") };

                return Task.Run(() => new HttpResponseMessage()
                {
                    Content = t.Content,
                    StatusCode = HttpStatusCode.OK
                }, cancellationToken);
            }
            return responseMessage;
        }
    }
}