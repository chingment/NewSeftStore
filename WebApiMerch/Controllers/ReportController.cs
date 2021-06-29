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
        public OwnApiHttpResponse StoreStockRealDataInit()
        {
            var result = MerchServiceFactory.Report.StoreStockRealDataInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreStockRealDataGet([FromBody]RopReportStoreStockRealDataGet rop)
        {
            var result = MerchServiceFactory.Report.StoreStockRealDataGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse StoreStockDateHisInit()
        {
            var result = MerchServiceFactory.Report.StoreStockDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreStockDateHisGet([FromBody]RopReporStoreStockDateHisGet rop)
        {
            var result = MerchServiceFactory.Report.StoreStockDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse SkuSalesDateHisInit()
        {
            var result = MerchServiceFactory.Report.SkuSalesDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SkuSalesDateHisGet([FromBody]RopReportSkuSalesDateHisGet rop)
        {
            var result = MerchServiceFactory.Report.SkuSalesDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse OrderSalesDateHisInit()
        {
            var result = MerchServiceFactory.Report.OrderSalesDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse OrderSalesDateHisGet([FromBody]RopReporOrderSalesDateHisGet rop)
        {
            var result = MerchServiceFactory.Report.OrderSalesDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse StoreSalesDateHisInit()
        {
            var result = MerchServiceFactory.Report.StoreSalesDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreSalesDateHisGet([FromBody]RopReporOrderSalesDateHisGet rop)
        {
            var result = MerchServiceFactory.Report.StoreSalesDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse CheckRightExport([FromBody]RopReportCheckRightExport rop)
        {
            var result = MerchServiceFactory.Report.CheckRightExport(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
