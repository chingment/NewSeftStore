using LocalS.Service.Api.StoreTerm;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiStoreTerm.Controllers
{
    [OwnApiAuthorize]
    public class ImServiceController : OwnApiBaseController
    {
        [HttpPost]
        public OwnApiHttpResponse UsersByIdle([FromBody]RopMachineInitData rop)
        {
            IResult result = StoreTermServiceFactory.Machine.InitData(rop);
            return new OwnApiHttpResponse(result);
        }
    }
}
