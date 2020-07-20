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
    public class PrdSubjectController : OwnApiBaseController
    {
        //[HttpGet]
        //public OwnApiHttpResponse GetList([FromUri]RupPrdSubjectGetList rup)
        //{
        //    IResult result = MerchServiceFactory.PrdSubject.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
        //    return new OwnApiHttpResponse(result);
        //}
        //[HttpGet]
        //public OwnApiHttpResponse InitAdd([FromUri]string pId)
        //{
        //    IResult result = MerchServiceFactory.PrdSubject.InitAdd(this.CurrentUserId, this.CurrentMerchId, pId);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpPost]
        //public OwnApiHttpResponse Add([FromBody]RopPrdSubjectAdd rop)
        //{
        //    IResult result = MerchServiceFactory.PrdSubject.Add(this.CurrentUserId, this.CurrentMerchId, rop);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpGet]
        //public OwnApiHttpResponse InitEdit([FromUri]string id)
        //{
        //    IResult result = MerchServiceFactory.PrdSubject.InitEdit(this.CurrentUserId, this.CurrentMerchId, id);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpPost]
        //public OwnApiHttpResponse Edit([FromBody]RopPrdSubjectEdit rop)
        //{
        //    IResult result = MerchServiceFactory.PrdSubject.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
        //    return new OwnApiHttpResponse(result);
        //}

        //[HttpPost]
        //public OwnApiHttpResponse Sort([FromBody]RopPrdSubjectSort rop)
        //{
        //    IResult result = MerchServiceFactory.PrdSubject.Sort(this.CurrentUserId, this.CurrentMerchId, rop);
        //    return new OwnApiHttpResponse(result);
        //}
    }
}
