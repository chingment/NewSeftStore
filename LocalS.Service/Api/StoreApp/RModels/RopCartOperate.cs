﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RopCartOperate
    {
        public string StoreId { get; set; }
        public E_CartOperateType Operate { get; set; }
        public List<SkuModel> Skus { get; set; }
        public E_ShopMode ShopMode { get; set; }
        public string ShopId { get; set; }
        public class SkuModel
        {
            public string Id { get; set; }
            public int Quantity { get; set; }
            public bool Selected { get; set; }
            public E_ShopMode ShopMode { get; set; }
            public string ShopId { get; set; }
        }
    }
}
