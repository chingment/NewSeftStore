using LocalS.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SellChannelStockLogChangeTpye
    {
        Unknow = 0,
        OrderReserveSuccess = 11,
        OrderCancle = 12,
        OrderPaySuccess = 13,
        OrderPickupOneSysMadeSignTake = 15,
        OrderPickupOneManMadeSignTakeByNotComplete = 16,
        OrderPickupOneManMadeSignNotTakeByComplete = 17,
        OrderPickupOneManMadeSignNotTakeByNotComplete = 18,
        SlotInit = 21,
        SlotEdit = 22,
        SlotRemove = 23
    }

    [Table("SellChannelStockLog")]
    public class SellChannelStockLog
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public string PrdProductSkuName { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public string SellChannelRefName { get; set; }
        public E_SellChannelStockLogChangeTpye ChangeType { get; set; }
        public string ChangeTypeName { get; set; }
        public int ChangeQuantity { get; set; }
        public int SumQuantity { get; set; }
        public int WaitPayLockQuantity { get; set; }
        public int WaitPickupLockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string RemarkByDev { get; set; }
    }
}
