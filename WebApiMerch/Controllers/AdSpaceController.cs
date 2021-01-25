using LocalS.Entity;
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
    public class AdSpaceController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupAdSpaceGetList rup)
        {
            IResult result = MerchServiceFactory.AdSpace.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitRelease([FromUri]E_AdSpaceId id)
        {
            IResult result = MerchServiceFactory.AdSpace.InitRelease(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Release([FromBody]RopAdSpaceRelease rop)
        {
            IResult result = MerchServiceFactory.AdSpace.Release(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetAdContents([FromUri]RupAdSpaceGetReleaseList rup)
        {
            IResult result = MerchServiceFactory.AdSpace.GetAdContents(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetAdContentBelongs([FromUri]RupAdSpaceGetAdContentBelongs rup)
        {
            IResult result = MerchServiceFactory.AdSpace.GetAdContentBelongs(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetAdContentBelongStatus([FromBody]RopAdSpaceSetAdContentBelongStatus rop)
        {
            IResult result = MerchServiceFactory.AdSpace.SetAdContentBelongStatus(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse CopyAdContent2Belongs([FromBody]RopAdContentCopy2Belongs rop)
        {
            IResult result = MerchServiceFactory.AdSpace.CopyAdContent2Belongs(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse DeleteAdContent([FromUri]string id)
        {
            IResult result = MerchServiceFactory.AdSpace.DeleteAdContent(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }
    }
}
