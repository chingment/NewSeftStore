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
    public class MerchMasterController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupMerchMasterGetList rup)
        {
            IResult result = AdminServiceFactory.MerchMaster.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            IResult result = AdminServiceFactory.MerchMaster.InitAdd(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopMerchMasterAdd rop)
        {
            IResult result = AdminServiceFactory.MerchMaster.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string userId)
        {
            IResult result = AdminServiceFactory.MerchMaster.InitEdit(this.CurrentUserId, userId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopMerchMasterEdit rop)
        {
            IResult result = AdminServiceFactory.MerchMaster.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}