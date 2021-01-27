using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("WxACode")]
    public class WxACode
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string AppId { get; set; }
        public string OpenId { get; set; }
        public string UserId { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public string ImgUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
