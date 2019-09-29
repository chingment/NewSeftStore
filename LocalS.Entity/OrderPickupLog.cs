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
        public string ProductSkuId { get; set; }
        public string SlotId { get; set; }
        public string EventCode { get; set; }
        public string EventRemark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
