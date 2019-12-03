using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopStockSettingTestPickupEventNotify
    {
        public string MachineId { get; set; }
        public string SlotId { get; set; }
        public string ProductSkuId { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int ActionStatusCode { get; set; }
        public string ActionStatusName { get; set; }
    }
}
