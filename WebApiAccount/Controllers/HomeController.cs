using LocalS.Service.Api.Account;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAccount.Controllers
{
    public class HomeController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetIndexPageData()
        {
            IResult result = AccountServiceFactory.Home.GetIndexPageData(this.CurrentUserId, this.CurrentUserId);

            return new OwnApiHttpResponse(result);
        }
    }
}