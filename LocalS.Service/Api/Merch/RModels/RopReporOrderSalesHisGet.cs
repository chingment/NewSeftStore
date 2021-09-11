using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReporOrderSalesHisGet:RupBaseGetList
    {
        public List<string> StoreIds { get; set; }

        public string[] TradeDateTimeArea { get; set; }

        public E_ReceiveMode ReceiveMode { get; set; }
    }
}
