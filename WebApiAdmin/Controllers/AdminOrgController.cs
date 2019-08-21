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
    public class AdminOrgController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupAdminOrgGetList rup)
        {
            IResult result = AdminServiceFactory.AdminOrg.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd([FromUri]string pId)
        {
            IResult result = AdminServiceFactory.AdminOrg.InitAdd(this.CurrentUserId, pId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopAdminOrgAdd rop)
        {
            IResult result = AdminServiceFactory.AdminOrg.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            IResult result = AdminServiceFactory.AdminOrg.InitEdit(this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopAdminOrgEdit rop)
        {
            IResult result = AdminServiceFactory.AdminOrg.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Sort([FromBody]RopAdminOrgSort rop)
        {
            IResult result = AdminServiceFactory.AdminOrg.Sort(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
