using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("OrderPickupLog")]
    public class OrderPickupLog
    {
        [Key]
        public string Id { get; set; }
        public string OrderId { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public string UniqueId { get; set; }
        public string PrdProductSkuId { get; set; }
        public string SlotId { get; set; }
        public E_OrderPickupStatus Status { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int ActionStatusCode { get; set; }
        public string ActionStatusName { get; set; }
        public string ActionRemark { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public int PickupUseTime { get; set; }
        public bool IsPickupComplete { get; set; }
        public string ImgId { get; set; }

    }
}
