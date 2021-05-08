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
            var result = AdminServiceFactory.MerchMaster.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            var result = AdminServiceFactory.MerchMaster.InitAdd(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopMerchMasterAdd rop)
        {
            var result = AdminServiceFactory.MerchMaster.Add(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            var result = AdminServiceFactory.MerchMaster.InitEdit(this.CurrentUserId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopMerchMasterEdit rop)
        {
            var result = AdminServiceFactory.MerchMaster.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}