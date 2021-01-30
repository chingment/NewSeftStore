using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class MachineProdcutSkuStockModel
    {
        public string ProductSkuId { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsOffSell { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public bool IsTrgVideoService { get; set; }
    }
}
