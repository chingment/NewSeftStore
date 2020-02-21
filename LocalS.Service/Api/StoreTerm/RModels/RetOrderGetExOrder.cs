﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderGetExOrder
    {
        public RetOrderGetExOrder()
        {
            this.ProductSkus = new List<ProductSku>();
        }
        public string OrderId { get; set; }

        public string OrderSn { get; set; }

        public List<ProductSku> ProductSkus { get; set; }

        public class ProductSku
        {
            public string Id { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string UniqueId { get; set; }
            public string SlotId { get; set; }

            public bool CanHandle { get; set; }
        }
    }
}