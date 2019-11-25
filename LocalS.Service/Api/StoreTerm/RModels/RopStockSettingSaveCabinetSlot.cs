using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopStockSettingSaveCabinetSlot
    {
        public string Id { get; set; }
        public string MachineId { get; set; }
        public string ProductSkuId { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int Version { get; set; }
    }
}
