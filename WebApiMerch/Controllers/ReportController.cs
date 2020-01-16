﻿using LocalS.Service.Api.Merch;
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
        public OwnApiHttpResponse MachineStockInit()
        {
            IResult result = MerchServiceFactory.Report.MachineStockInit(this.CurrentUserId,this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse MachineStockGet([FromBody]RopReportMachineStockGet rop)
        {
            IResult result = MerchServiceFactory.Report.MachineStockGet(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse ProductSkuDaySalesInit()
        {
            IResult result = MerchServiceFactory.Report.ProductSkuDaySalesInit(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse ProductSkuDaySalesGet([FromBody]RopReportProductSkuDaySalesGet rup)
        {
            IResult result = MerchServiceFactory.Report.ProductSkuDaySalesGet(this.CurrentUserId, this.CurrentMerchId,rup);
            return new OwnApiHttpResponse(result);
        }

    }
}
