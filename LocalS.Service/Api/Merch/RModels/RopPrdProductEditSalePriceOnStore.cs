using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPrdProductEditSalePriceOnStore
    {
        public string StoreId { get; set; }

        public string ProductSkuId { get; set; }

        public decimal ProductSkuSalePrice { get; set; }

        public bool ProductSkuIsOffSell { get; set; }
    }
}
