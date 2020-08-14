using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupPayRefundGetList : RupBaseGetList
    {
        public string PayRefundId { get; set; }

        public string OrderId { get; set; }

        public string PayTransId { get; set; }

        public string PayPartnerOrderId { get; set; }
    }
}
