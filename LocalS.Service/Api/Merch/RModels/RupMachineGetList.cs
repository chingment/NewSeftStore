﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupMachineGetList:RupBaseGetList
    {
        public string Id { get; set; }

        public string StoreId { get; set; }

        public string ShopId { get; set; }

        public string OpCode { get; set; }
    }
}
