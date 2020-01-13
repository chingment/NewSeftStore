using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupReportProductSkuDaySalesGet
    {
        public string StoreId { get; set; }
        public string MachineId { get; set; }

        public string[] TradeDateTimeArea { get; set; }
    }
}
