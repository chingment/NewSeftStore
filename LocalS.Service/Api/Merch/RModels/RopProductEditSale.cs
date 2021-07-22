using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopProductEditSale
    {
        public string StoreId { get; set; }

        public string SkuId { get; set; }

        public decimal SalePrice { get; set; }

        public bool IsOffSell { get; set; }
    }
}
