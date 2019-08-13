using LocalS.Service.Api.InsApp;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApiInsApp.Controllers
{
    public class InsCarController : OwnApiBaseController
    {
        [HttpGet]
        public OwnApiHttpResponse GetIndexPageData()
        {
            IResult result = InsAppServiceFactory.InsCar.GetIndexPageData();

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse SearchPlateNoInfo([FromUri]RupInsCarSearchPlateNoInfo rup)
        {
            IResult result = InsAppServiceFactory.InsCar.SearchPlateNoInfo(rup);

            return new OwnApiHttpResponse(result);
        }

        [HttpGet]
        public OwnApiHttpResponse SearchModelInfo([FromUri]RupInsCarSearchModelInfo rup)
        {
            IResult result = InsAppServiceFactory.InsCar.SearchModelInfo(rup);

            return new OwnApiHttpResponse(result);
        }
    }
}
