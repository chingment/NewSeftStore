using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_UniqueType
    {
        Unknow = 0,
        Order = 1,
        OrderSub = 2
    }

    public enum E_ShopMode
    {
        Unknow = 0,
        Mall = 1,//商城库存
        Shop = 2,//门店库存
        Machine = 3,//机器库存
    }

    public enum E_ReceiveMode
    {
        Unknow = 0,
        Delivery = 1,
        SelfTakeByStore = 2,
        SelfTakeByMachine = 4,
        FeeByMember = 5,
        ConsumeByStore = 6,
        FeeByDeposit = 7,
        FeeByRent = 8
    }

    [Table("SellChannelStock")]
    public class SellChannelStock
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public E_ShopMode ShopMode { get; set; }
        public string ShopId { get; set; }
        public string MachineId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public string SpuId { get; set; }
        public string SkuId { get; set; }
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
