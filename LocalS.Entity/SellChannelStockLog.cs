using LocalS.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SellChannelStockLogChangeTpye
    {
        Unknow = 0,
        ReserveSuccess = 1,
        OrderPaySuccess = 2,
        OrderCancle = 3,
        SlotInit = 4,
        SlotEdit = 5,
        SlotRemove = 6
    }

    [Table("SellChannelStockLog")]
    public class SellChannelStockLog
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }

        public string StoreId { get; set; }
        public string SlotId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public E_SellChannelRefType RefType { get; set; }
        public string RefId { get; set; }
        public E_SellChannelStockLogChangeTpye ChangeType { get; set; }
        public int ChangeQuantity { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string RemarkByDev { get; set; }
    }
}
