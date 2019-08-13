using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAdmin.Controllers
{
    public class HomeController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetIndexPageData([FromUri]string mId, [FromUri]string uId)
        {
            return new OwnApiHttpResponse(null);
        }
    }
}