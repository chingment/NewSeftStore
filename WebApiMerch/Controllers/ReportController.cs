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
    public class ReportController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse DeviceStockRealDataInit()
        {
            var result = MerchServiceFactory.Report.DeviceStockRealDataInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceStockRealDataGet([FromBody]RopReportStoreStockRealDataGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceStockRealDataGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse DeviceReplenishPlanInit()
        {
            var result = MerchServiceFactory.Report.DeviceReplenishPlanInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceReplenishPlanGet([FromBody]RopReportDeviceReplenishPlanGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceReplenishPlanGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse DeviceStockDateHisInit()
        {
            var result = MerchServiceFactory.Report.DeviceStockDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceStockDateHisGet([FromBody]RopReporStoreStockDateHisGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceStockDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse SkuSalesHisInit()
        {
            var result = MerchServiceFactory.Report.SkuSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SkuSalesHisGet([FromBody]RopReportSkuSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.SkuSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse OrderSalesHisInit()
        {
            var result = MerchServiceFactory.Report.OrderSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse OrderSalesHisGet([FromBody]RopReporOrderSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.OrderSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse StoreSalesHisInit()
        {
            var result = MerchServiceFactory.Report.StoreSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreSalesHisGet([FromBody]RopReporStoreSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.StoreSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse CheckRightExport([FromBody]RopReportCheckRightExport rop)
        {
            var result = MerchServiceFactory.Report.CheckRightExport(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse DeviceSalesHisInit()
        {
            var result = MerchServiceFactory.Report.DeviceSalesHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse DeviceSalesHisGet([FromBody]RopReportDeviceSalesHisGet rop)
        {
            var result = MerchServiceFactory.Report.DeviceSalesHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
