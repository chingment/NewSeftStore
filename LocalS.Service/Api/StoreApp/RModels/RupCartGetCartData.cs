﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupCartGetCartData
    {
        public string StoreId { get; set; }

        public string ShopId { get; set; }
        public E_ShopMode ShopMode { get; set; }

    }
}
