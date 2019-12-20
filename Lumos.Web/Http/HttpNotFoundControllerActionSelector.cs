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
    public class HttpNotFoundControllerActionSelector : ApiControllerActionSelector
    {
        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            HttpActionDescriptor decriptor = null;
            try
            {
                decriptor = base.SelectAction(controllerContext);
            }
            catch (HttpResponseException ex)
            {
                var code = ex.Response.StatusCode;

                if (code == HttpStatusCode.NotFound || code == HttpStatusCode.MethodNotAllowed)
                {
                    var result = new CustomJsonResult(ResultType.Exception, ResultCode.Exception, "无效请求");

                    var t = new HttpResponseMessage { Content = new StringContent(result.ToString(), Encoding.GetEncoding("UTF-8"), "application/json") };
                    ex.Response.Content = t.Content;
                }
                ex.Response.StatusCode = HttpStatusCode.OK;
                throw;
            }
            return decriptor;
        }
    }
}
