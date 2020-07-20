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
    public class PrdKindController : OwnApiBaseController
    {
        //[HttpGet]
        //public OwnApiHttpResponse GetList([FromUri]RupPrdKindGetList rup)
        //{
        //    IResult result = MerchServiceFactory.PrdKind.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
        //    return new OwnApiHttpResponse(result);
        //}
        //[HttpGet]
        //public OwnApiHttpResponse InitAdd([FromUri]string pId)
        //{
        //    IResult result = MerchServiceFactory.PrdKind.InitAdd(this.CurrentUserId, this.CurrentMerchId, pId);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpPost]
        //public OwnApiHttpResponse Add([FromBody]RopPrdKindAdd rop)
        //{
        //    IResult result = MerchServiceFactory.PrdKind.Add(this.CurrentUserId, this.CurrentMerchId, rop);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpGet]
        //public OwnApiHttpResponse InitEdit([FromUri]string id)
        //{
        //    IResult result = MerchServiceFactory.PrdKind.InitEdit(this.CurrentUserId, this.CurrentMerchId, id);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpPost]
        //public OwnApiHttpResponse Edit([FromBody]RopPrdKindEdit rop)
        //{
        //    IResult result = MerchServiceFactory.PrdKind.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpPost]
        //public OwnApiHttpResponse Sort([FromBody]RopPrdKindSort rop)
        //{
        //    IResult result = MerchServiceFactory.PrdKind.Sort(this.CurrentUserId, this.CurrentMerchId, rop);
        //    return new OwnApiHttpResponse(result);
        //}
    }
}
