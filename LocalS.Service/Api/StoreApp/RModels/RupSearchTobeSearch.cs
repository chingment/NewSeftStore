﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupSearchTobeSearch
    {
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public E_SellChannelRefType ShopMode { get; set; }
        public E_OrderShopMethod ShopMethod { get; set; }
        public string Key { get; set; }
    }
}
