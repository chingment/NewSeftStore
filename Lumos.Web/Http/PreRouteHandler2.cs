using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace Lumos.Web.Http
{
    public static class PreRouteHandler2
    {
        public static void HttpPreRoute2(this HttpConfiguration config)
        {
            config.Services.Replace(typeof(IHttpActionInvoker), new HttpWebApiControllerActionInvoker2());
            config.Services.Replace(typeof(IHttpControllerSelector), new HttpNotFoundControllerSelector2(config));
            config.Services.Replace(typeof(IHttpActionSelector), new HttpNotFoundControllerActionSelector2());
        }
    }
}
