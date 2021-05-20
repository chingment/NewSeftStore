using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{

    public enum E_MemberFeeSt_FeeType
    {
        Unknow = 0,
        LongTerm = 1,//终身
        TwelveMonth = 2,//12个月 即年
        SixMonth = 3,//6个月   即半年
        ThreeMonth = 4,//3个月   即季度
        OneMonth = 5 //1个月  
    }

    [Table("MemberFeeSt")]
    public class MemberFeeSt
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public string Tag { get; set; }
        public string MainImgUrl { get; set; }
        public string LevelStId { get; set; }
        public E_MemberFeeSt_FeeType FeeType { get; set; }
        public decimal FeeOriginalValue { get; set; }
        public decimal FeeSaleValue { get; set; }
        public string LayoutWeight { get; set; }
        public string DesPoints { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }

        //public string CouponIds { get; set; }
        public bool IsStop { get; set; }
    }
}
