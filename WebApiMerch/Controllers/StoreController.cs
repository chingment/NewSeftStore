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
        public OwnApiHttpResponse InitAdd()
        {
            IResult result = MerchServiceFactory.Store.InitAdd(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopStoreAdd rop)
        {
            IResult result = MerchServiceFactory.Store.Add(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopStoreEdit rop)
        {
            IResult result = MerchServiceFactory.Store.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
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
        public OwnApiHttpResponse InitManageMachine([FromUri]string id)
        {
            IResult result = MerchServiceFactory.Store.InitManageMachine(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetMachineList([FromUri]RupStoreGetMachineList rup)
        {
            IResult result = MerchServiceFactory.Store.GetMachineList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse AddMachine([FromBody]RopStoreAddMachine rop)
        {
            IResult result = MerchServiceFactory.Store.AddMachine(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RemoveMachine([FromBody]RopStoreRemoveMachine rop)
        {
            IResult result = MerchServiceFactory.Store.RemoveMachine(this.CurrentUserId, this.CurrentMerchId, rop);
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
        public OwnApiHttpResponse GetKindSpuInfo([FromUri]RupStoreGetKindSpu rup)
        {
            IResult result = MerchServiceFactory.Store.GetKindSpuInfo(this.CurrentUserId, this.CurrentMerchId, rup);
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
        public OwnApiHttpResponse InitManageFront([FromUri]string id)
        {
            IResult result = MerchServiceFactory.Store.InitManageMachine(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetFrontList([FromUri]RupStoreGetFrontList rup)
        {
            IResult result = MerchServiceFactory.Store.GetFrontList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetFront([FromUri]RupStoreGetFront rup)
        {
            IResult result = MerchServiceFactory.Store.GetFront(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveFront([FromBody]RopStoreSaveFront rop)
        {
            IResult result = MerchServiceFactory.Store.SaveFront(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}