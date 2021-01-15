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
        public E_ShopMode ShopMode { get; set; }
        public E_UniqueType UniqueType { get; set; }
        public string UniqueId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public string MachineId { get; set; }
        public string CabinetId { get; set; }
        public string PrdProductSkuId { get; set; }
        public string SlotId { get; set; }
        public E_OrderPickupStatus Status { get; set; }
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int ActionStatusCode { get; set; }
        public string ActionStatusName { get; set; }
        public string ActionRemark { get; set; }
        public DateTime? ActionTime { get; set; }
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public int PickupUseTime { get; set; }
        public string ImgId { get; set; }
        public string ImgId2 { get; set; }
    }
}
