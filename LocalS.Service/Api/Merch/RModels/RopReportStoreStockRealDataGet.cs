﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReportStoreStockRealDataGet : RupBaseGetList
    {
        public string DeviceId { get; set; }
        public string[] TradeDateArea { get; set; }
    }
}
