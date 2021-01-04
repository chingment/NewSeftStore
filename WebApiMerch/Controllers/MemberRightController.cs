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
    }
}
