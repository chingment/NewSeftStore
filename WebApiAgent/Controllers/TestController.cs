using LocalS.Service.Api.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAgent.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public OwnApiHttpResponse Index()
        {


            AgentServiceFactory.User.GetList("e5d1a2ca4883474791ca91ce20c90014", "", new RupUserGetList { });
            return new OwnApiHttpResponse(null);
        }
    }
}
