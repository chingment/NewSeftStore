using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupDeviceGetStocks : RupBaseGetList
    {
        public string DeviceId { get; set; }
        public string CabinetId { get; set; }
        public string SkuName { get; set; }
    }
}
