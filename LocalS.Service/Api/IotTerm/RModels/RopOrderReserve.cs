using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class RopOrderReserve
    {
        public string device_id { get; set; }
        public string low_order_id { get; set; }
        public bool is_im_ship { get; set; }
        public string notify_url { get; set; }
        public List<Detail> detail { get; set; }

        public class Detail
        {
            public string sku_id { get; set; }

            public string sku_cum_code { get; set; }

            public int quantity { get; set; }
        }
    }
}
