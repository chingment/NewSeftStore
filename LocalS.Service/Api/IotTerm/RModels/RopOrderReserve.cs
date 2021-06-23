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
        public List<object> detail { get; set; }
    }
}
