using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SellChannelStock")]
    public class SellChannelStock
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public E_SellChannelRefType RefType { get; set; }
        public string RefId { get; set; }
        public string SlotId { get; set; }
        public int SumQuantity { get; set; }
        public int WaitPayLockQuantity { get; set; }
        public int WaitPickupLockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int MaxLimitSumQuantity { get; set; }
        public bool IsOffSell { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SalePriceByVip { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }

        public int Version { get; set; }
    }
}
