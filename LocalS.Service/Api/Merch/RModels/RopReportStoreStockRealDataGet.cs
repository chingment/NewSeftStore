using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopReportStoreStockRealDataGet
    {
        public List<string> StoreIds { get; set; }

        public E_ShopMode ShopMode { get; set; }

    }
}
