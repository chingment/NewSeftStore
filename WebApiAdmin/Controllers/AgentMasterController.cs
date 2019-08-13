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
            IResult result = AdminServiceFactory.AgentMaster.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            IResult result = AdminServiceFactory.AgentMaster.InitAdd(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopAgentMasterAdd rop)
        {
            IResult result = AdminServiceFactory.AgentMaster.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string userId)
        {
            IResult result = AdminServiceFactory.AgentMaster.InitEdit(this.CurrentUserId, userId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopAgentMasterEdit rop)
        {
            IResult result = AdminServiceFactory.AgentMaster.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}