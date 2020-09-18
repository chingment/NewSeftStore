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
    public class MachineController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse InitGetList()
        {
            IResult result = MerchServiceFactory.Machine.InitGetList(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupMachineGetList rup)
        {
            IResult result = MerchServiceFactory.Machine.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManage([FromUri]string id)
        {
            IResult result = MerchServiceFactory.Machine.InitManage(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageBaseInfo([FromUri]string id)
        {
            IResult result = MerchServiceFactory.Machine.InitManageBaseInfo(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitManageStock([FromUri]string id)
        {
            IResult result = MerchServiceFactory.Machine.InitManageStock(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse ManageStockGetStocks([FromUri]RupMachineGetStocks rup)
        {
            IResult result = MerchServiceFactory.Machine.ManageStockGetStocks(this.CurrentUserId, this.CurrentMerchId, rup.MachineId, rup.CabinetId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ManageStockEditStock([FromBody]RopMachineEditStock rop)
        {
            IResult result = MerchServiceFactory.Machine.ManageStockEditStock(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopMachineEdit rop)
        {
            IResult result = MerchServiceFactory.Machine.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SysReboot([FromBody]RopMachineRebootSys rop)
        {
            //SdkFactory.Wx.GiftvoucherActivityNotifyPick("dad", "otakHv019rDPK-sMjbBUj8khGgAE", "1212122122", "test", "33311231", "test", DateTime.Now, "http://www.17fanju.com");

            IResult result = MerchServiceFactory.Machine.SysReboot(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SysShutdown([FromBody]RopMachineShutdownSys rop)
        {
            IResult result = MerchServiceFactory.Machine.SysShutdown(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SysSetStatus([FromBody]RopMachineSetSysStatus rop)
        {
            IResult result = MerchServiceFactory.Machine.SysSetStatus(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        public OwnApiHttpResponse Dsx01OpenPickupDoor([FromBody]RopMachineShutdownSys rop)
        {
            IResult result = MerchServiceFactory.Machine.Dsx01OpenPickupDoor(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse QueryMsgPushResult([FromBody]RopMachineQueryMsgPushResult rop)
        {
            IResult result = MerchServiceFactory.Machine.QueryMsgPushResult(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
