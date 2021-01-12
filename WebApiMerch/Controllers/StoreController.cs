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
    public class StoreController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupStoreGetList rup)
        {
            IResult result = MerchServiceFactory.Store.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManage([FromUri]string id = "")
        {
            IResult result = MerchServiceFactory.Store.InitManage(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageBaseInfo([FromUri]string id)
        {
            IResult result = MerchServiceFactory.Store.InitManageBaseInfo(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMachines([FromUri]RupStoreGetMachines rup)
        {
            IResult result = MerchServiceFactory.Store.GetMachines(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetKinds(string id)
        {
            IResult result = MerchServiceFactory.Store.GetKinds(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveKind([FromBody]RopStoreSaveKind rop)
        {
            IResult result = MerchServiceFactory.Store.SaveKind(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RemoveKind([FromBody]RopStoreRemoveKind rop)
        {
            IResult result = MerchServiceFactory.Store.RemoveKind(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveKindSpu([FromBody]RopStoreSaveKindSpu rop)
        {
            IResult result = MerchServiceFactory.Store.SaveKindSpu(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetKindSpu([FromUri]RupStoreGetKindSpu rup)
        {
            IResult result = MerchServiceFactory.Store.GetKindSpu(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RemoveKindSpu([FromBody]RopStoreSaveKindSpu rop)
        {
            IResult result = MerchServiceFactory.Store.RemoveKindSpu(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetKindSpus([FromUri]RupStoreGetKindSpus rup)
        {
            IResult result = MerchServiceFactory.Store.GetKindSpus(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageShop([FromUri]string id)
        {
            IResult result = MerchServiceFactory.Store.InitManageShop(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetShops([FromUri]RupStoreGetShops rup)
        {
            IResult result = MerchServiceFactory.Store.GetShops(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}