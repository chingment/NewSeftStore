using LocalS.Service.Api.StoreTerm;
using Lumos;
using System.IO;
using System.Web;
using System.Web.Http;


namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class StockSettingController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetCabinetSlots([FromUri]RupStockSettingGetCabinetSlots rup)
        {
            IResult result = StoreTermServiceFactory.StockSetting.GetCabinetSlots("",rup);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveCabinetSlot([FromBody]RopStockSettingSaveCabinetSlot rop)
        {
            IResult result = StoreTermServiceFactory.StockSetting.SaveCabinetSlot("", rop);
            return new OwnApiHttpResponse(result);
        }

        [HttpPost]
        public OwnApiHttpResponse SaveCabinetRowColLayout([FromBody]RopStockSettingSaveCabinetRowColLayout rop)
        {
            IResult result = StoreTermServiceFactory.StockSetting.SaveCabinetRowColLayout("", rop);
            return new OwnApiHttpResponse(result);
        }

    }
}
