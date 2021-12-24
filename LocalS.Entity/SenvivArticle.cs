using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SenvivArticleType
    {
        Unknow = 0,
        SolarTerm = 1,
        Pregnancy = 2
    }

    [Table("SenvivArticle")]
    public class SenvivArticle
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public E_SenvivArticleType Type{ get; set; }
        public string TypeValue { get; set; }
        public string Content { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
