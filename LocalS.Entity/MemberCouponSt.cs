using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("MemberCouponSt")]
    public class MemberCouponSt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string LevelStId { get; set; }
        public string CouponId { get; set; }
        public int Quantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
