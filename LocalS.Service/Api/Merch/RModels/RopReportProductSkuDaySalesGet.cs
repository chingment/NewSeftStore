using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReportProductSkuDaySalesGet
    {
        public List<string[]> SellChannels { get; set; }

        public string[] TradeDateTimeArea { get; set; }

        public string PickupStatus { get; set; }
    }
}
