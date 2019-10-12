using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopMachineEditStock
    {
        public string MachineId { get; set; }
        public string ProductSkuId { get; set; }
        public string SlotId { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsOffSell { get; set; }
    }
}
