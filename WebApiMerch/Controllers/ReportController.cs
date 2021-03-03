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
            IResult result = MerchServiceFactory.Report.StoreStockRealDataInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreStockRealDataGet([FromBody]RopReportMachineStockRealDataGet rop)
        {
            IResult result = MerchServiceFactory.Report.StoreStockRealDataGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse StoreStockDateHisInit()
        {
            IResult result = MerchServiceFactory.Report.StoreStockDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreStockDateHisGet([FromBody]RopReportMachineStockDateHisGet rop)
        {
            IResult result = MerchServiceFactory.Report.StoreStockDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse SkuSalesDateHisInit()
        {
            IResult result = MerchServiceFactory.Report.SkuSalesDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SkuSalesDateHisGet([FromBody]RopReportSkuSalesDateHisGet rop)
        {
            IResult result = MerchServiceFactory.Report.SkuSalesDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse OrderSalesDateHisInit()
        {
            IResult result = MerchServiceFactory.Report.OrderSalesDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse OrderSalesDateHisGet([FromBody]RopReporOrderSalesDateHisGet rop)
        {
            IResult result = MerchServiceFactory.Report.OrderSalesDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse StoreSalesDateHisInit()
        {
            IResult result = MerchServiceFactory.Report.StoreSalesDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse StoreSalesDateHisGet([FromBody]RopReporOrderSalesDateHisGet rop)
        {
            IResult result = MerchServiceFactory.Report.StoreSalesDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
