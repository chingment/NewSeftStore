using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPayRefundApply
    {
        public string OrderId { get; set; }
        public string Method { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
    }
}
