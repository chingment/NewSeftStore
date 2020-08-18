using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupPayTransGetList:RupBaseGetList
    {
        public string PayTransId { get; set; }

        public string OrderId { get; set; }

        public string PayPartnerPayTransId { get; set; }

        public string ClientUserName { get; set; }

        public string StoreId { get; set; }
    }
}
