using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SellChannelStockDateHis")]
    public class SellChannelStockDateHis
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public int SumQuantity { get; set; }
        public int WaitPayLockQuantity { get; set; }
        public int WaitPickupLockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public bool IsOffSell { get; set; }
        public decimal SalePrice { get; set; }
        public string StockDate{ get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public int Version { get; set; }
    }
}
