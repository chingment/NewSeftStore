using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RopOrderReserve
    {
        public string StoreId { get; set; }
        public List<Product> Products { get; set; }

        public E_OrderSource Source { get; set; }

        public class Product
        {
            public string Id { get; set; }
            public string CartId { get; set; }
            public int Quantity { get; set; }

            public E_ReceptionMode ReceptionMode { get; set; }
        }
    }
}
