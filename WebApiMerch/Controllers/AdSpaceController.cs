﻿using LocalS.Entity;
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
        public OwnApiHttpResponse GetReleaseList([FromUri]RupAdSpaceGetReleaseList rup)
        {
            IResult result = MerchServiceFactory.AdSpace.GetReleaseList(this.CurrentUserId, this.CurrentMerchId, rup);
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
        public OwnApiHttpResponse DeleteAdContent([FromUri]string id)
        {
            IResult result = MerchServiceFactory.AdSpace.DeleteAdContent(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }
    }
}