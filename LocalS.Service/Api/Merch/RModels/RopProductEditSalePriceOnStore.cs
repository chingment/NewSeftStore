using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopProductEditSalePriceOnStore
    {
        public string StoreId { get; set; }

        public string SkuId { get; set; }

        public decimal SkuSalePrice { get; set; }

        public bool SkuIsOffSell { get; set; }
    }
}
