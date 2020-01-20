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
            IResult result = MerchServiceFactory.Home.GetIndexPageData(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetTodaySummary()
        {
            IResult result = MerchServiceFactory.Home.GetTodaySummary(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse Get7DayGmv()
        {
            IResult result = MerchServiceFactory.Home.Get7DayGmv(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse GetTodayStoreGmvRl()
        {
            IResult result = MerchServiceFactory.Home.GetTodayStoreGmvRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetStoreGmvRl()
        {
            IResult result = MerchServiceFactory.Home.GetStoreGmvRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetProductSkuSaleRl()
        {
            IResult result = MerchServiceFactory.Home.GetProductSkuSaleRl(this.CurrentUserId, this.CurrentMerchId);

            return new OwnApiHttpResponse(result);
        }
    }
}