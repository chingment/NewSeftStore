﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopMachineEditStock
    {
        public string MachineId { get; set; }
        public string SkuId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public int SumQuantity { get; set; }
        public int Version { get; set; }
    }
}
