using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("MemberFeeSt")]
    public class MemberFeeSt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string LevelStId { get; set; }
        public int FeeType { get; set; }
        public decimal FeeValue { get; set; }
        public string LayoutWeight { get; set; }
        public string DesPoints { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
