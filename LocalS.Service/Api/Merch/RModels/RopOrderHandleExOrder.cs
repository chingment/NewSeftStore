using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopOrderHandleExOrder
    {
        public RopOrderHandleExOrder()
        {
            this.DetailItems = new List<DetailItem>();
        }

        public string OrderId { get; set; }

        public List<DetailItem> DetailItems { get; set; }

        public class DetailItem
        {
            public string Id { get; set; }
            public string UniqueId { get; set; }
            public int PickupStatus { get; set; }
        }

    }
}
