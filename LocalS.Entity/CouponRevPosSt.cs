using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("CouponRevPosSt")]
    public class CouponRevPosSt
    {
        [Key]
        public string Id { get; set; }
        public string Code { get; set; }
        public string MerchId { get; set; }
        public string TopImgUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
