using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderPickupStatusQuery
    {
        public RetOrderPickupStatusQuery()
        {
            this.ProductSkus = new List<ProductSku>();
        }

        public List<ProductSku> ProductSkus { get; set; }
        public class ProductSku
        {
            public ProductSku()
            {
                this.Slots = new List<Slot>();
            }

            public string Id { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public int QuantityBySuccess { get; set; }
            public int QuantityByException { get; set; }
            public List<Slot> Slots { get; set; }
        }
        public class Slot
        {
            public string UniqueId { get; set; }
            public string SlotId { get; set; }
            public E_OrderDetailsChildSonStatus Status { get; set; }
        }
    }
}
