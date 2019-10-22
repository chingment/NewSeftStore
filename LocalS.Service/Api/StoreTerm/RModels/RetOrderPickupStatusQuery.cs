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
        public string ProductSkuId { get; set; }
        public string UniqueId { get; set; }
        public string SlotId { get; set; }
        public E_OrderDetailsChildSonStatus Status { get; set; }
    }
}
