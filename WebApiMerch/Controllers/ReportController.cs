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
        public OwnApiHttpResponse MachineStockRealDataInit()
        {
            IResult result = MerchServiceFactory.Report.MachineStockRealDataInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse MachineStockRealDataGet([FromBody]RopReportMachineStockRealDataGet rop)
        {
            IResult result = MerchServiceFactory.Report.MachineStockRealDataGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse MachineStockDateHisInit()
        {
            IResult result = MerchServiceFactory.Report.MachineStockDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse MachineStockDateHisGet([FromBody]RopReportMachineStockDateHisGet rop)
        {
            IResult result = MerchServiceFactory.Report.MachineStockDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse ProductSkuSalesDateHisInit()
        {
            IResult result = MerchServiceFactory.Report.ProductSkuSalesDateHisInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ProductSkuSalesDateHisGet([FromBody]RopReportProductSkuSalesDateHisGet rop)
        {
            IResult result = MerchServiceFactory.Report.ProductSkuSalesDateHisGet(this.CurrentUserId, this.CurrentMerchId, rop);
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

    }
}
