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
    public class ClientUserController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupClientGetList rup)
        {
            IResult result = MerchServiceFactory.ClientUser.GetList(this.CurrentUserId, this.CurrentMerchId, rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitDetails([FromUri]string id)
        {
            IResult result = MerchServiceFactory.ClientUser.InitDetails(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitDetailsBaseInfo([FromUri]string id)
        {
            IResult result = MerchServiceFactory.ClientUser.InitDetailsBaseInfo(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse InitDetailsOrders([FromUri]string id)
        {
            IResult result = MerchServiceFactory.ClientUser.InitDetailsOrders(this.CurrentUserId, this.CurrentMerchId, id);
            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse DetailsOrdersGetOrderList([FromUri]RupClientDetailsOrdersGetOrderList rup)
        {
            IResult result = MerchServiceFactory.ClientUser.DetailsOrdersGetOrderList(this.CurrentUserId, this.CurrentMerchId,rup);
            return new OwnApiHttpResponse(result);
        }


        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopClientUserEdit rop)
        {
            IResult result = MerchServiceFactory.ClientUser.Edit(this.CurrentUserId, this.CurrentMerchId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
