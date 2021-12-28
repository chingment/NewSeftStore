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
        public OwnApiHttpResponse GetDayReportDetail([FromUri]string reportId, [FromUri]string taskId)
        {
            var result = MerchServiceFactory.Senviv.GetDayReportDetail(this.CurrentUserId, this.CurrentMerchId, reportId, taskId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetStageReports([FromUri]RupSenvivGetDayReports rup)
        {
            var result = MerchServiceFactory.Senviv.GetStageReports(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetStageReportDetail([FromUri]string reportId, [FromUri]string taskId)
        {
            var result = MerchServiceFactory.Senviv.GetStageReportDetail(this.CurrentUserId, this.CurrentMerchId, reportId, taskId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetStageReportSug([FromUri]string reportId)
        {
            var result = MerchServiceFactory.Senviv.GetStageReportSug(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveStageReportSug([FromBody]SenvivSaveMonthReportSug rop)
        {
            var result = MerchServiceFactory.Senviv.SaveStageReportSug(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDayReportSug([FromUri]string reportId)
        {
            var result = MerchServiceFactory.Senviv.GetDayReportSug(this.CurrentUserId, this.CurrentMerchId, reportId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveDayReportSug([FromBody]SenvivSaveMonthReportSug rop)
        {
            var result = MerchServiceFactory.Senviv.SaveDayReportSug(this.CurrentUserId, this.CurrentMerchId, rop);
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
        public OwnApiHttpResponse GetTasks([FromUri]RupSenvivGetTasks rup)
        {
            var result = MerchServiceFactory.Senviv.GetTasks(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetHandleRecords([FromUri]RupSenvivGetVisitRecords rup)
        {
            var result = MerchServiceFactory.Senviv.GetVisitRecords(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetArticles([FromUri]RupSenvivGetTasks rup)
        {
            var result = MerchServiceFactory.Senviv.GetArticles(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetArticle([FromUri]string id)
        {
            var result = MerchServiceFactory.Senviv.GetArticle(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveArticle([FromBody]RopSenvivSaveArticle rop)
        {
            var result = MerchServiceFactory.Senviv.SaveArticle(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


    }
}
