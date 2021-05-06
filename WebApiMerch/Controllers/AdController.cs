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
    public class AdController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetSpaces([FromUri]RupAdGetSpaces rup)
        {
            var result = MerchServiceFactory.Ad.GetSpaces(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitRelease([FromUri]E_AdSpaceId adSpaceId)
        {
            var result = MerchServiceFactory.Ad.InitRelease(this.CurrentUserId, this.CurrentMerchId, adSpaceId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Release([FromBody]RopAdRelease rop)
        {
            var result = MerchServiceFactory.Ad.Release(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetContents([FromUri]RupAdGetContents rup)
        {
            var result = MerchServiceFactory.Ad.GetContents(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse InitContents([FromUri]E_AdSpaceId adSpaceId)
        {
            var result = MerchServiceFactory.Ad.InitContents(this.CurrentUserId, this.CurrentMerchId, adSpaceId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitBelongs([FromUri]string adContentId)
        {
            IResult result = MerchServiceFactory.Ad.InitBelongs(this.CurrentUserId, this.CurrentMerchId, adContentId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetSelBelongs([FromUri]string adContentId)
        {
            var result = MerchServiceFactory.Ad.GetSelBelongs(this.CurrentUserId, this.CurrentMerchId, adContentId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetContentStatus([FromBody]RopAdSetContentStatus rop)
        {
            var result = MerchServiceFactory.Ad.SetContentStatus(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetContentBelongs([FromUri]RupAdGetContentBelongs rup)
        {
            var result = MerchServiceFactory.Ad.GetContentBelongs(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetContentBelongStatus([FromBody]RopAdSetAdContentBelongStatus rop)
        {
            var result = MerchServiceFactory.Ad.SetContentBelongStatus(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse AddContentBelong([FromBody]RopAdAddContentBelong rop)
        {
            var result = MerchServiceFactory.Ad.AddContentBelong(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse EditContentBelong([FromBody]RopAdEditContentBelong rop)
        {
            var result = MerchServiceFactory.Ad.EditContentBelong(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
