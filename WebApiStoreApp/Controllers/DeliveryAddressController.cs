using LocalS.Service.Api.StoreApp;
using Lumos;
using System.Web.Http;

namespace WebApiStoreApp.Controllers
{
    public class DeliveryAddressController : OwnApiBaseController
    {

        [HttpGet]
        public OwnApiHttpResponse My(RupDeliveryAddressMy rup)
        {
            var result = StoreAppServiceFactory.DeliveryAddress.My(this.CurrentUserId, this.CurrentUserId, rup);
    
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse Edit([FromBody]RopDeliveryAddressEdit rop)
        {
            var result = StoreAppServiceFactory.DeliveryAddress.Edit(this.CurrentUserId, this.CurrentUserId, rop);
            return new OwnApiHttpResponse(result);
        }
    }
}