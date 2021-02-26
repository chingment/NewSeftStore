using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("CouponRevCouponSt")]
    public class CouponRevCouponSt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string RevPosId { get; set; }
        public string CouponId { get; set; }
        public int PerLimitNum { get; set; }
        public E_Coupon_PerLimitTimeType PerLimitTimeType { get; set; }
        public int PerLimitTimeNum { get; set; }
        public string LimitMemberLevels { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
