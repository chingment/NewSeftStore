﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class OrderConfirmSkuModel : SkuModel
    {
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public decimal SalePriceByVip { get; set; }
        public E_ReceptionMode ReceptionMode { get; set; }
    }
}
