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
    public class AgentMasterController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupAgentMasterGetList rup)
        {
            var result = AdminServiceFactory.AgentMaster.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            var result = AdminServiceFactory.AgentMaster.InitAdd(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopAgentMasterAdd rop)
        {
            var result = AdminServiceFactory.AgentMaster.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            var result = AdminServiceFactory.AgentMaster.InitEdit(this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopAgentMasterEdit rop)
        {
            var result = AdminServiceFactory.AgentMaster.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}