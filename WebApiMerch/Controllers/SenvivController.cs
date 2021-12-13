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
            var result = MerchServiceFactory.Senviv.GetUsers(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetUserDetail([FromUri]string userId)
        {
            var result = MerchServiceFactory.Senviv.GetUserDetail(this.CurrentUserId, this.CurrentMerchId, userId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDayReports([FromUri]RupSenvivGetDayReports rup)
        {
            var result = MerchServiceFactory.Senviv.GetDayReports(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDayReportDetail([FromUri]string reportId)
        {
            var result = MerchServiceFactory.Senviv.GetDayReportDetail(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMonthReports([FromUri]RupSenvivGetDayReports rup)
        {
            var result = MerchServiceFactory.Senviv.GetMonthReports(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMonthReportDetail([FromUri]string reportId)
        {
            var result = MerchServiceFactory.Senviv.GetMonthReportDetail(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMonthReportSug([FromUri]string reportId)
        {
            var result = MerchServiceFactory.Senviv.GetMonthReportSug(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveMonthReportSug([FromBody]SenvivSaveMonthReportSug rop)
        {
            var result = MerchServiceFactory.Senviv.SaveMonthReportSug(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetTagExplains([FromUri]RupSenvivGetTags rup)
        {
            var result = MerchServiceFactory.Senviv.GetTagExplains(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveTagExplain([FromBody]RopSenvivSaveTagExplain rop)
        {
            var result = MerchServiceFactory.Senviv.SaveTagExplain(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetVisitRecords([FromUri]RupSenvivGetVisitRecords rup)
        {
            var result = MerchServiceFactory.Senviv.GetVisitRecords(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveVisitRecordByTelePhone([FromBody]RopSenvivSaveVisitRecordByTelePhone rop)
        {
            var result = MerchServiceFactory.Senviv.SaveVisitRecordByTelePhone(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
        [HttpPost]
        public OwnApiHttpResponse SaveVisitRecordByPapush([FromBody]RopSenvivSaveVisitRecordByPapush rop)
        {
            var result = MerchServiceFactory.Senviv.SaveVisitRecordByPapush(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetTasks([FromUri]RupSenvivGetUsers rup)
        {
            var result = MerchServiceFactory.Senviv.GetTasks(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}
