using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductSkuStockModel
    {
        public E_ShopMode ShopMode { get; set; }
        public string ShopId { get; set; }
        public string MachineId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public bool IsOffSell { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsUseRent { get; set; }
        public decimal RentMhPrice { get; set; }
        public decimal DepositPrice { get; set; }
    }
}
