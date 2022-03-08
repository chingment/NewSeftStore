using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class ArticleController : ApiController
    {
        [HttpGet]
        public OwnApiHttpResponse Details([FromUri]string id, [FromUri] string svuid)
        {
            var result = HealthAppServiceFactory.Article.Details("", id, svuid);
            return new OwnApiHttpResponse(result);
        }


    }
}
