﻿using LocalS.Service.Api.Merch;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiMerch.Controllers
{
    public class HomeController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetInitData()
        {
            var result = MerchServiceFactory.Home.GetInitData(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

    }
}