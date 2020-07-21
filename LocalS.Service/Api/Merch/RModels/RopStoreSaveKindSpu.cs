using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopStoreSaveKindSpu
    {
        public string StoreId { get; set; }
        public string KindId { get; set; }
        public string ProductId { get; set; }
        public List<StockInfo> Stocks { get; set; }

        public bool IsSellMall { get; set; }

        public class StockInfo
        {
            public string SkuId { get; set; }
            public decimal SalePrice { get; set; }
            public int SumQuantity { get; set; }

            public bool IsOffSell { get; set; }
        }
    }
}
