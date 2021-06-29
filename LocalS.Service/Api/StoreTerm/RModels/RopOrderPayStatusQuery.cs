using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopOrderPayStatusQuery
    {
        public string DeviceId { get; set; }
        public string PayTransId { get; set; }
        public string OrderId { get; set; }
    }
}
