using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{

    [Table("MemberDaySt")]
    public class MemberDaySt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string Day { get; set; }
        public int MemberLevel { get; set; }
        public decimal Discount { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get;  set; }
        public DateTime? MendTime { get; set; }
    }
}
