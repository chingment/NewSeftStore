using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalS.Entity;

namespace LocalS.BLL.Biz
{
    public class StockChangeRecordModel
    {
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public string MachineId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public string SkuId { get; set; }
        public E_ShopMode ShopMode { get; set; }
        public string EventCode { get; set; }
        public int ChangeQuantity { get; set; }
        public int SumQuantity { get; set; }
        public int WaitPayLockQuantity { get; set; }
        public int WaitPickupLockQuantity { get; set; }
        public int SellQuantity { get; set; }
    }
}
