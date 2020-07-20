using LocalS.Service.Api.Admin;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAdmin.Controllers
{
    public class MerchPrdKindController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupPrdKindGetList rup)
        {
            IResult result = AdminServiceFactory.MerchPrdKind.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd([FromUri]int pId)
        {
            IResult result = AdminServiceFactory.MerchPrdKind.InitAdd(this.CurrentUserId, pId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopPrdKindAdd rop)
        {
            IResult result = AdminServiceFactory.MerchPrdKind.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]int id)
        {
            IResult result = AdminServiceFactory.MerchPrdKind.InitEdit(this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopPrdKindEdit rop)
        {
            IResult result = AdminServiceFactory.MerchPrdKind.Edit(this.CurrentUserId,rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Sort([FromBody]RopPrdKindSort rop)
        {
            IResult result = AdminServiceFactory.MerchPrdKind.Sort(this.CurrentUserId,rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
