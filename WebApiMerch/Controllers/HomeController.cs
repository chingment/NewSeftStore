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
    public class HomeController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse GetIndexPageData()
        {
            var result = MerchServiceFactory.Home.GetIndexPageData(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetTodaySummary()
        {
            var result = MerchServiceFactory.Home.GetTodaySummary(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Get7DayGmv()
        {
            var result = MerchServiceFactory.Home.Get7DayGmv(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse GetTodayStoreGmvRl()
        {
            var result = MerchServiceFactory.Home.GetTodayStoreGmvRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetStoreGmvRl()
        {
            var result = MerchServiceFactory.Home.GetStoreGmvRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetSkuSaleRl()
        {
            var result = MerchServiceFactory.Home.GetSkuSaleRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }
    }
}