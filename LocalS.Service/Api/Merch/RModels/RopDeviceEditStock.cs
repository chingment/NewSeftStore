using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopDeviceEditStock
    {
        public string DeviceId { get; set; }
        public string SkuId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public int SumQuantity { get; set; }
        public int Version { get; set; }
        public int MaxQuantity { get; set; }
        public int WarnQuantity { get; set; }
        public int HoldQuantity { get; set; }
    }
}
