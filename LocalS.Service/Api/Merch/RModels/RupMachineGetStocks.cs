using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupMachineGetStocks : RupBaseGetList
    {
        public string MachineId { get; set; }
        public string CabinetId { get; set; }
        public string ProductSkuName { get; set; }
    }
}
