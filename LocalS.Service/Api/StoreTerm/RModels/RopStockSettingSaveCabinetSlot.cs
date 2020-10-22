using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopStockSettingSaveCabinetSlot
    {
        public string SlotId { get; set; }
        public string StockId { get; set; }
        public string CabinetId { get; set; }
        public string MachineId { get; set; }
        public string ProductSkuId { get; set; }
        public int SumQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public int WarnQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public int Version { get; set; }
    }
}
