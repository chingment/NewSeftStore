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
    public class ProductController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupPrdProductGetList rup)
        {
            var result = MerchServiceFactory.PrdProduct.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitAdd()
        {
            var result = MerchServiceFactory.PrdProduct.InitAdd(this.CurrentUserId, this.CurrentMerchId);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Add([FromBody]RopProductAdd rop)
        {
            var result = MerchServiceFactory.PrdProduct.Add(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitEdit([FromUri]string id)
        {
            var result = MerchServiceFactory.PrdProduct.InitEdit(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopProductEdit rop)
        {
            var result = MerchServiceFactory.PrdProduct.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }


        [HttpGet]
        public OwnApiHttpResponse GetOnSaleStores([FromUri]string id)
        {
            var result = MerchServiceFactory.PrdProduct.GetOnSaleStores(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse EditSalePriceOnStore([FromBody]RopProductEditSalePriceOnStore rop)
        {
            var result = MerchServiceFactory.PrdProduct.EditSalePriceOnStore(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse SearchSpu(string key)
        {

            var result = MerchServiceFactory.PrdProduct.SearchSpu(this.CurrentUserId, this.CurrentMerchId, key);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse SearchSku(string key)
        {

            var result = MerchServiceFactory.PrdProduct.SearchSku(this.CurrentUserId, this.CurrentMerchId, key);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse GetSpecs(string id)
        {
            var result = MerchServiceFactory.PrdProduct.GetSpecs(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }
    }
}