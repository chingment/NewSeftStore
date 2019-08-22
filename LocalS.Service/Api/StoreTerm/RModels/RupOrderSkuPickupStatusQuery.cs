using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RupOrderSkuPickupStatusQuery
    {
        public string MachineId { get; set; }
        public string OrderId { get; set; }
        public string UniqueId { get; set; }
        public string SkuId { get; set; }
        public string SlotId { get; set; }
    }
}
