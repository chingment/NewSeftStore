using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class RopDeviceList
    {
        public int page { get; set; }
        public int limit { get; set; }

        public string device_id { get; set; }
    }
}
