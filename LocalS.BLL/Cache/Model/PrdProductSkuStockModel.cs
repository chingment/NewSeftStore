using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class PrdProductSkuStockModel
    {
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public E_StoreSellChannelRefType RefType { get; set; }
        public string RefId { get; set; }
        public string SlotId { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public bool IsOffSell { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SalePriceByVip { get; set; }
    }
}
