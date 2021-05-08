﻿using LocalS.BLL.Biz;
using LocalS.Service.Api.StoreTerm;
using Lumos;
using System.IO;
using System.Web;
using System.Web.Http;


namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class StockSettingController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetCabinetSlots([FromUri]RupStockSettingGetCabinetSlots rup)
        {
            var result = StoreTermServiceFactory.StockSetting.GetCabinetSlots(this.CurrentUserId,rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse<RetOperateSlot> SaveCabinetSlot([FromBody]RopStockSettingSaveCabinetSlot rop)
        {
            var result = StoreTermServiceFactory.StockSetting.SaveCabinetSlot(this.CurrentUserId, rop);
            return new OwnApiHttpResponse<RetOperateSlot>(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveCabinetRowColLayout([FromBody]RopStockSettingSaveCabinetRowColLayout rop)
        {
            var result = StoreTermServiceFactory.StockSetting.SaveCabinetRowColLayout(this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
