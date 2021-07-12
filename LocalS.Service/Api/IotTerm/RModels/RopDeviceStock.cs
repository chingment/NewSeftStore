using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class RopDeviceStock
    {
        public string device_id { get; set; }
        public string device_cum_code { get; set; }
        public string data_format { get; set; }
        public string cabinet_id { get; set; }
        public bool is_need_detail { get; set; }
    }
}
