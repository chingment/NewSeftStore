using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Lumos.Web.Http
{
    public class HttpNotFoundControllerSelector : DefaultHttpControllerSelector
    {
        public HttpNotFoundControllerSelector(HttpConfiguration configuration)
        : base(configuration)
        {
        }
        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            HttpControllerDescriptor decriptor = null;
            try
            {
                decriptor = base.SelectController(request);
            }
            catch (HttpResponseException ex)
            {
                var httpStatusCode = ex.Response.StatusCode;

                var result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "请求异常");
                if (httpStatusCode == HttpStatusCode.NotFound)
                {
                    result.Message = "无效请求.";
                }
                else if (httpStatusCode == HttpStatusCode.MethodNotAllowed)
                {
                    result.Message = "方法不允许访问";
                }

                var httpResponseMessage = new HttpResponseMessage { Content = new StringContent(result.ToString(), Encoding.GetEncoding("UTF-8"), "application/json") };
                ex.Response.Content = httpResponseMessage.Content;
                ex.Response.StatusCode = HttpStatusCode.OK;
                throw;
            }
            return decriptor;
        }


    }
}
