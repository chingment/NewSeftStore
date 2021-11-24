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
    public class ShopWorkBenchController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetInitData()
        {
            var result = MerchServiceFactory.ShopWorkBench.GetInitData(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetTodaySummary()
        {
            var result = MerchServiceFactory.ShopWorkBench.GetTodaySummary(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Get7DayGmv()
        {
            var result = MerchServiceFactory.ShopWorkBench.Get7DayGmv(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse GetTodayStoreGmvRl()
        {
            var result = MerchServiceFactory.ShopWorkBench.GetTodayStoreGmvRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetStoreGmvRl()
        {
            var result = MerchServiceFactory.ShopWorkBench.GetStoreGmvRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetSkuSaleRl()
        {
            var result = MerchServiceFactory.ShopWorkBench.GetSkuSaleRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }
    }
}
