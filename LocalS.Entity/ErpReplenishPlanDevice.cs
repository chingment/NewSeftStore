using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{

    [Table("ErpReplenishPlanDevice")]
    public class ErpReplenishPlanDevice
    {
        [Key]
        public string Id { get; set; }
        public string PlanId { get; set; }
        public string PlanCumCode { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCumCode { get; set; }
        public string Cabinets { get; set; }
        public string MakerName { get; set; }
        public string MakerId { get; set; }
        public string MakeDate { get; set; }
        public DateTime MakeTime { get; set; }
        public DateTime? RshTime { get; set; }
        public string RsherId { get; set; }
        public string RsherName { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }

    }
}
