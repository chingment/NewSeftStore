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
    public class LogController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse InitListByOperate()
        {
            var result = MerchServiceFactory.Log.InitListByOperate(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListByOperate([FromUri]RupLogGetListByOperate rup)
        {
            var result = MerchServiceFactory.Log.GetListByOperate(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitListByStock()
        {
            var result = MerchServiceFactory.Log.InitListByStock(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListByStock([FromUri]RupLogGetListByStock rup)
        {
            var result = MerchServiceFactory.Log.GetListByStock(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListByRelStock([FromUri]RupLogGetListByRelStock rup)
        {
            var result = MerchServiceFactory.Log.GetListByRelStock(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}
