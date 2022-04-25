using LocalS.BLL;
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
    public class DevSenvivLiteController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse InitGetList()
        {
            var result = MerchServiceFactory.DevSenvivLite.InitGetList(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupDeviceGetList rup)
        {
            var result = MerchServiceFactory.DevSenvivLite.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UnBindMerch([FromBody]RopDeviceUnBindMerch rop)
        {
            var result = MerchServiceFactory.DevSenvivLite.UnBindMerch(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UnBindUser([FromBody]RopDeviceUnBindMerch rop)
        {
            var result = MerchServiceFactory.DevSenvivLite.UnBindMerch(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindMerch([FromBody]RopDeviceBindMerch rop)
        {
            var result = MerchServiceFactory.DevSenvivLite.BindMerch(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
