﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderSkuPickupStatusQuery
    {
        public E_OrderDetailsChildSonStatus Status { get; set; }

        public string Tips { get; set; }
    }
}
