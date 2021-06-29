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
            var result = MerchServiceFactory.Store.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManage([FromUri]string id = "")
        {
            var result = MerchServiceFactory.Store.InitManage(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageBaseInfo([FromUri]string id)
        {
            var result = MerchServiceFactory.Store.InitManageBaseInfo(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetDevices([FromUri]RupStoreGetDevices rup)
        {
            var result = MerchServiceFactory.Store.GetDevices(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetKinds(string id)
        {
            var result = MerchServiceFactory.Store.GetKinds(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveKind([FromBody]RopStoreSaveKind rop)
        {
            var result = MerchServiceFactory.Store.SaveKind(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RemoveKind([FromBody]RopStoreRemoveKind rop)
        {
            var result = MerchServiceFactory.Store.RemoveKind(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveKindSpu([FromBody]RopStoreSaveKindSpu rop)
        {
            var result = MerchServiceFactory.Store.SaveKindSpu(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetKindSpu([FromUri]RupStoreGetKindSpu rup)
        {
            var result = MerchServiceFactory.Store.GetKindSpu(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RemoveKindSpu([FromBody]RopStoreSaveKindSpu rop)
        {
            var result = MerchServiceFactory.Store.RemoveKindSpu(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetKindSpus([FromUri]RupStoreGetKindSpus rup)
        {
            var result = MerchServiceFactory.Store.GetKindSpus(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageShop([FromUri]string id)
        {
            var result = MerchServiceFactory.Store.InitManageShop(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetShops([FromUri]RupStoreGetShops rup)
        {
            var result = MerchServiceFactory.Store.GetShops(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        public OwnApiHttpResponse RemoveShop([FromBody]RopStoreRemoveShop rop)
        {
            var result = MerchServiceFactory.Store.RemoveShop(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse AddShop([FromBody]RopStoreAddShop rop)
        {
            var result = MerchServiceFactory.Store.AddShop(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}