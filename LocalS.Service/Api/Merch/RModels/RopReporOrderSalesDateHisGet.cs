using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReporOrderSalesDateHisGet
    {
        public List<string> StoreIds { get; set; }

        public string[] TradeDateTimeArea { get; set; }
    }
}
