﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class RopOrderReserve
    {
        public RopOrderReserve()
        {
            this.ProductSkus = new List<ProductSku>();
        }

        public string StoreId { get; set; }
        public string ClientUserId { get; set; }
        public E_OrderSource Source { get; set; }
        public E_ReserveMode ReserveMode { get; set; }
        public string SellChannelRefId { get; set; }
        public E_StoreSellChannelRefType SellChannelRefType { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceptionAddress { get; set; }
        public List<ProductSku> ProductSkus { get; set; }
        public class ProductSku
        {
            public string CartId { get; set; }
            public string Id { get; set; }
            public int Quantity { get; set; }

            public E_ReceptionMode ReceptionMode { get; set; }
        }
    }
}
