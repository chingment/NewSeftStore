using LocalS.Service.Api.Merch;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiMerch.Controllers
{
    public class SenvivController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetUsers([FromUri]RupSenvivGetUsers rup)
        {
            IResult result = MerchServiceFactory.Senviv.GetUsers(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetUserDetail([FromUri]string userId)
        {
            IResult result = MerchServiceFactory.Senviv.GetUserDetail(this.CurrentUserId, this.CurrentMerchId, userId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDayReports([FromUri]RupSenvivGetDayReports rup)
        {
            IResult result = MerchServiceFactory.Senviv.GetDayReports(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDayReportDetail([FromUri]string reportId)
        {
            IResult result = MerchServiceFactory.Senviv.GetDayReportDetail(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMonthReports([FromUri]RupSenvivGetDayReports rup)
        {
            IResult result = MerchServiceFactory.Senviv.GetMonthReports(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMonthReportDetail([FromUri]string reportId)
        {
            IResult result = MerchServiceFactory.Senviv.GetMonthReportDetail(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMonthReportSug([FromUri]string reportId)
        {
            IResult result = MerchServiceFactory.Senviv.GetMonthReportSug(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveMonthReportSug([FromBody]SenvivSaveMonthReportSug rop)
        {
            IResult result = MerchServiceFactory.Senviv.SaveMonthReportSug(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
