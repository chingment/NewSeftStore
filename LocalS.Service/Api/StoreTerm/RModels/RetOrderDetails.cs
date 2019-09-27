using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderDetails
    {
        public RetOrderDetails()
        {
            this.Skus = new List<Sku>();
        }
        public string CsrQrCode { get; set; }
        public string Sn { get; set; }
        public List<Sku> Skus { get; set; }
        public class Sku
        {
            public Sku()
            {
                this.Slots = new List<Slot>();
            }

            public string Id { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public int QuantityBySuccess { get; set; }

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
