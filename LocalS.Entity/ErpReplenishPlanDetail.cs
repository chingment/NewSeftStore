using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("ErpReplenishPlanDetail")]
    public class ErpReplenishPlanDetail
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
        public string SpuId { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public string SkuCumCode { get; set; }
        public string SkuSpecDes { get; set; }
        public string DeviceId { get; set; }
        public string DeviceCumCode { get; set; }
        public int PlanRshQuantity { get; set; }
        public int RealRshQuantity { get; set; }
        public DateTime? RshTime { get; set; }
        public string RsherId { get; set; }
        public string RsherName { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
