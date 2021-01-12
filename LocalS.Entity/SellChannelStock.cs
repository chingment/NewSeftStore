using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SellChannelStock")]
    public class SellChannelStock
    {

        public const string MallSellChannelRefId = "000000000000";
        public const string MemberFeeSellChannelRefId = "000000000001";

        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public int SumQuantity { get; set; }
        public int WaitPayLockQuantity { get; set; }
        public int WaitPickupLockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public int WarnQuantity { get; set; }
        public int HoldQuantity { get; set; }
        public bool IsOffSell { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SevsPrice { get; set; }
        public bool IsUseRent { get; set; }
        public decimal RentMhPrice { get; set; }
        public decimal DepositPrice { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public int Version { get; set; }
    }
}
