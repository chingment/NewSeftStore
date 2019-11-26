using LocalS.Service.Api.Admin;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiAdmin.Controllers
{
    public class MerchMachineController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetList([FromUri]RupMerchMachineGetList rup)
        {
            IResult result = AdminServiceFactory.MerchMachine.GetList(this.CurrentUserId, rup);
            return new OwnApiHttpResponse(result);
        }
    }
}
