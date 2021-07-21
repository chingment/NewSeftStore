using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReportDeviceReplenishPlanGet
    {
        public string PlanId { get; set; }
        public string MakerName { get; set; }
        public string SkuCumCode { get; set; }
        public string ShopName { get; set; }
    }
}
