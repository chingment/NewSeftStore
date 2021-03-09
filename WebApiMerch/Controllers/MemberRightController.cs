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
    public class MemberRightController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetLevelSts()
        {
            IResult result = MerchServiceFactory.MemberRight.GetLevelSts(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManage([FromUri]string id = "")
        {
            IResult result = MerchServiceFactory.MemberRight.InitManage(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetLevelSt([FromUri]string id)
        {
            IResult result = MerchServiceFactory.MemberRight.GetLevelSt(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetLevelSt([FromBody]RopMemberRightSetLevelSt rop)
        {
            IResult result = MerchServiceFactory.MemberRight.SetLevelSt(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetFeeSts([FromUri]string id = "")
        {
            IResult result = MerchServiceFactory.MemberRight.GetFeeSts(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetFeeSt([FromBody]RopMemberRightSetFeeSt rop)
        {
            IResult result = MerchServiceFactory.MemberRight.SetFeeSt(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetCoupons([FromUri]RupMemberRightGetCoupons rup)
        {
            IResult result = MerchServiceFactory.MemberRight.GetCoupons(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RemoveCoupon([FromBody]RopMemberRightRemoveCoupon rop)
        {
            IResult result = MerchServiceFactory.MemberRight.RemoveCoupon(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse AddCoupon([FromBody]RopMemberRightAddCoupon rop)
        {
            IResult result = MerchServiceFactory.MemberRight.AddCoupon(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse SearchCoupon(string key)
        {

            IResult result = MerchServiceFactory.Coupon.Search(this.CurrentUserId, this.CurrentMerchId, key);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetSkus([FromUri]RupMemberRightGetSkus rup)
        {
            IResult result = MerchServiceFactory.MemberRight.GetSkus(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse AddSku([FromBody]RopMemberRightAddSku rop)
        {
            IResult result = MerchServiceFactory.MemberRight.AddSku(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse EditSku([FromBody]RopMemberRightEditSku rop)
        {
            IResult result = MerchServiceFactory.MemberRight.EditSku(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
