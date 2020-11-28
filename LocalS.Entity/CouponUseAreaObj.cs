using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("CouponUseAreaObj")]
    public class CouponUseAreaObj
    {
        [Key]
        public string Id { get; set; }
        public string CouponId { get; set; }
        public string ObjId { get; set; }
        public string ObjName { get; set; }
        public string ObjType { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
