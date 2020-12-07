using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("MemberRightSt")]
    public class MemberBagSt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string LevelId { get; set; }
        public string FeeId { get; set; }
        public string RightType { get; set; }
        public string RightValue { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
