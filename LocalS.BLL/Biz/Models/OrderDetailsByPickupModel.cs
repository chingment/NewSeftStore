using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class OrderProductSkuByPickupModel
    {
        public OrderProductSkuByPickupModel()
        {
            this.Slots = new List<Slot>();
        }

        public string ProductSkuId { get; set; }
        public string MainImgUrl { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int QuantityBySuccess { get; set; }
        public int QuantityByException { get; set; }
        public List<Slot> Slots { get; set; }

        public class Slot
        {
            public string UniqueId { get; set; }
            public string CabinetId { get; set; }
            public string SlotId { get; set; }
            public E_OrderPickupStatus Status { get; set; }
            public bool IsAllowPickup { get; set; }
        }
    }
}
