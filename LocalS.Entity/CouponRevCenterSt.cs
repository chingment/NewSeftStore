using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("CouponRevCenterSt")]
    public class CouponRevCenterSt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string CouponIds { get; set; }
        public string TopImgUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
