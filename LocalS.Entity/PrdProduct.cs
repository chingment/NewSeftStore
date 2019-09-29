using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("PrdProduct")]
    public class PrdProduct
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string PrdKindIds { get; set; }
        public string PrdSubjectIds { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string PinYinName{ get; set; }
        public string PinYinIndex { get; set; }
        public string MainImgUrl { get; set; }
        public string DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? MendTime { get; set; }
        public string Mender { get; set; }
    }
}
