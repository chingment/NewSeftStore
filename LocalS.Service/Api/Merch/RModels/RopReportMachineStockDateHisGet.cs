using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReportMachineStockDateHisGet
    {
        public List<string> StoreIds { get; set; }

        public String StockDate { get; set; }

        public E_SellChannelRefType SellChannelRefType { get; set; }
    }
}
