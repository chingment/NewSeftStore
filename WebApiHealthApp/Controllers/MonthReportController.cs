using LocalS.Service.Api.HealthApp;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiHealthApp.Controllers
{
    public class MonthReportController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetMonitor([FromUri]string rptId)
        {
            var result = HealthAppServiceFactory.MonthReport.GetMonitor(this.CurrentUserId, rptId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetEnergy([FromUri]string rptId)
        {
            var result = HealthAppServiceFactory.MonthReport.GetEnergy(this.CurrentUserId, rptId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetAdvise([FromUri]string rptId)
        {
            var result = HealthAppServiceFactory.MonthReport.GetAdvise(this.CurrentUserId, rptId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetTagAdvise([FromUri]string tagId)
        {
            var result = HealthAppServiceFactory.MonthReport.GetTagAdvise(this.CurrentUserId, tagId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse UpdateVisitCount([FromUri]string rptId)
        {
            var result = HealthAppServiceFactory.MonthReport.UpdateVisitCount(this.CurrentUserId, rptId);
            return new OwnApiHttpResponse(result);
        }
    }
}
