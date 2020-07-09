using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.DbRelay
{
    [Table("SysMerchUser")]
    public class SysMerchUser : SysUser
    {
        public string MerchId { get; set; }
        public bool IsMaster { get; set; }
        public bool ImIsUse { get; set; }
        public string ImPartner { get; set; }
        public string ImUserName { get; set; }
        public string ImPassword { get; set; }
        public string CharTags { get; set; }
        public string BriefDes { get; set; }
    }
}
