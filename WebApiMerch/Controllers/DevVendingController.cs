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
    public class DevVendingController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse InitGetList()
        {
            var result = MerchServiceFactory.DevVending.InitGetList(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupDeviceGetList rup)
        {
            var result = MerchServiceFactory.DevVending.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListByShop([FromUri]RupDeviceGetList rup)
        {
            var result = MerchServiceFactory.DevVending.GetListByShop(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetListBySbShop([FromUri]RupDeviceGetList rup)
        {
            var result = MerchServiceFactory.DevVending.GetListBySbShop(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse InitManage([FromUri]string id = "")
        {
            var result = MerchServiceFactory.DevVending.InitManage(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageBaseInfo([FromUri]string id)
        {
            var result = MerchServiceFactory.DevVending.InitManageBaseInfo(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageStock([FromUri]string id)
        {
            var result = MerchServiceFactory.DevVending.InitManageStock(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse ManageStockGetStocks([FromUri]RupDeviceGetStocks rup)
        {
            var result = MerchServiceFactory.DevVending.ManageStockGetStocks(this.CurrentUserId, this.CurrentMerchId, rup.DeviceId, rup.CabinetId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ManageStockEditStock([FromBody]RopDeviceEditStock rop)
        {
            var result = MerchServiceFactory.DevVending.ManageStockEditStock(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopDeviceEdit rop)
        {
            var result = MerchServiceFactory.DevVending.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse RebootSys([FromBody]RopDeviceRebootSys rop)
        {
            var result = MerchServiceFactory.DevVending.RebootSys(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ShutdownSys([FromBody]RopDeviceShutdownSys rop)
        {
            var result = MerchServiceFactory.DevVending.ShutdownSys(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetSysStatus([FromBody]RopDeviceSetSysStatus rop)
        {
            var result = MerchServiceFactory.DevVending.SetSysStatus(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetSysParams([FromUri]string id)
        {
            var result = MerchServiceFactory.DevVending.GetSysParams(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SetSysParams([FromBody]RopDeviceSetSysParams rop)
        {
            var result = MerchServiceFactory.DevVending.SetSysParams(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        

        [HttpPost]
        public OwnApiHttpResponse OpenPickupDoor([FromBody]RopDeviceOpenPickupDoor rop)
        {
            var result = MerchServiceFactory.DevVending.OpenPickupDoor(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UpdateApp([FromBody]RopDeviceOpenPickupDoor rop)
        {
            var result = MerchServiceFactory.DevVending.UpdateApp(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse UnBindShop([FromBody]RopDeviceUnBindShop rop)
        {
            var result = MerchServiceFactory.DevVending.UnBindShop(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse BindShop([FromBody]RopDeviceUnBindShop rop)
        {
            var result = MerchServiceFactory.DevVending.BindShop(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
