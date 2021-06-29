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
    public class MerchDeviceController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse InitGetList()
        {
            var result = AdminServiceFactory.MerchDevice.InitGetList(this.CurrentUserId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupMerchDeviceGetList rup)
        {
            var result = AdminServiceFactory.MerchDevice.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]RupMerchDeviceInitEdit rup)
        {
            var result = AdminServiceFactory.MerchDevice.InitEdit(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopMerchDeviceEdit rop)
        {
            var result = AdminServiceFactory.MerchDevice.Edit(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindOffMerch([FromBody]RopMerchDeviceBindOffMerch rop)
        {
            var result = AdminServiceFactory.MerchDevice.BindOffMerch(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindOnMerch([FromBody]RopMerchDeviceBindOnMerch rop)
        {
            var result = AdminServiceFactory.MerchDevice.BindOnMerch(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
