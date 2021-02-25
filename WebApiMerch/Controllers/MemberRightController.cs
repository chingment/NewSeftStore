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
        public OwnApiHttpResponse InitManageBaseInfo([FromUri]string id)
        {
            IResult result = MerchServiceFactory.MemberRight.InitManageBaseInfo(this.CurrentUserId, this.CurrentMerchId, id);
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
        public OwnApiHttpResponse GetCouponsByLevelSt([FromUri]RupMemberRightGetLevelCoupons rup)
        {
            IResult result = MerchServiceFactory.MemberRight.GetCouponsByLevelSt(this.CurrentUserId, this.CurrentMerchId, rup);
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


    }
}
