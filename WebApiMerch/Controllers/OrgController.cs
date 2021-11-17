using LocalS.Service.Api.Merch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiMerch.Controllers
{
    public class OrgController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupOrgGetList rup)
        {
            var result = MerchServiceFactory.Org.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd([FromUri]string pId)
        {
            var result = MerchServiceFactory.Org.InitAdd(this.CurrentUserId, this.CurrentMerchId, pId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopOrgAdd rop)
        {
            var result = MerchServiceFactory.Org.Add(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            var result = MerchServiceFactory.Org.InitEdit(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopOrgEdit rop)
        {
            var result = MerchServiceFactory.Org.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Sort([FromBody]RopOrgSort rop)
        {
            var result = MerchServiceFactory.Org.Sort(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
