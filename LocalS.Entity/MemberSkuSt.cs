using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("MemberSkuSt")]
    public class MemberSkuSt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string LevelStId { get; set; }
        public string StoreId { get; set; }
        public int MemberLevel { get; set; }
        public decimal MemberPrice { get; set; }
        public string SkuId { get; set; }
        public bool IsDisabled { get; set; }
        public DateTime StatTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
