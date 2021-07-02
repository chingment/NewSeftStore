using LocalS.BLL;
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
    public class DeviceController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse InitGetList()
        {
            var result = MerchServiceFactory.Device.InitGetList(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupDeviceGetList rup)
        {
            var result = MerchServiceFactory.Device.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManage([FromUri]string id = "")
        {
            var result = MerchServiceFactory.Device.InitManage(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageBaseInfo([FromUri]string id)
        {
            var result = MerchServiceFactory.Device.InitManageBaseInfo(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageStock([FromUri]string id)
        {
            var result = MerchServiceFactory.Device.InitManageStock(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse ManageStockGetStocks([FromUri]RupDeviceGetStocks rup)
        {
            var result = MerchServiceFactory.Device.ManageStockGetStocks(this.CurrentUserId, this.CurrentMerchId, rup.DeviceId, rup.CabinetId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ManageStockEditStock([FromBody]RopDeviceEditStock rop)
        {
            var result = MerchServiceFactory.Device.ManageStockEditStock(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopDeviceEdit rop)
        {
            var result = MerchServiceFactory.Device.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RebootSys([FromBody]RopDeviceRebootSys rop)
        {
            var result = MerchServiceFactory.Device.RebootSys(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ShutdownSys([FromBody]RopDeviceShutdownSys rop)
        {
            var result = MerchServiceFactory.Device.ShutdownSys(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetSysStatus([FromBody]RopDeviceSetSysStatus rop)
        {
            var result = MerchServiceFactory.Device.SetSysStatus(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        public OwnApiHttpResponse OpenPickupDoor([FromBody]RopDeviceOpenPickupDoor rop)
        {
            var result = MerchServiceFactory.Device.OpenPickupDoor(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse QueryMsgPushResult([FromBody]RopDeviceQueryMsgPushResult rop)
        {
            var result = MerchServiceFactory.Device.QueryMsgPushResult(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UnBindShop([FromBody]RopDeviceUnBindShop rop)
        {
            var result = MerchServiceFactory.Device.UnBindShop(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindShop([FromBody]RopDeviceUnBindShop rop)
        {
            var result = MerchServiceFactory.Device.BindShop(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
