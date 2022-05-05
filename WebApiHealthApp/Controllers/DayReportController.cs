﻿using LocalS.Service.Api.HealthApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class DayReportController : OwnApiBaseController
    {
        [HttpGet]
        [AllowAnonymous]
        public OwnApiHttpResponse GetDetails([FromUri]string rptId)
        {
            var result = HealthAppServiceFactory.DayReport.GetDetails(this.CurrentUserId, rptId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public OwnApiHttpResponse GetIndicator([FromUri]string rptId)
        {
            var result = HealthAppServiceFactory.DayReport.GetIndicator(this.CurrentUserId, rptId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public OwnApiHttpResponse UpdateVisitCount([FromUri]string rptId)
        {
            var result = HealthAppServiceFactory.DayReport.UpdateVisitCount(this.CurrentUserId, rptId);
            return new OwnApiHttpResponse(result);
        }
    }
}
