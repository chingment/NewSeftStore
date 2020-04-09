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
            this.UniqueItems = new List<UniqueItem>();
        }

        public string Id { get; set; }
        public List<UniqueItem> UniqueItems { get; set; }
        public string Remark { get; set; }
        public class UniqueItem
        {
            public string UniqueId { get; set; }
            public int SignStatus { get; set; }
        }

    }
}
