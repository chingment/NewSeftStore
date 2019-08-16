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
    }
}
