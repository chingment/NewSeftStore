﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{


    public class RopOrderReserve
    {
        public RopOrderReserve()
        {
            this.ProductSkus = new List<ProductSku>();
        }

        public string MachineId { get; set; }
        public List<ProductSku> ProductSkus { get; set; }
        public class ProductSku
        {
            public string SkuId { get; set; }
            public int Quantity { get; set; }
            public string SvcConsulterId { get; set; }
        }
    }
}
