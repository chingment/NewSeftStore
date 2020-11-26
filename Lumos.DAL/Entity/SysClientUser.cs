using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.DbRelay
{
    [Table("SysClientUser")]
    public class SysClientUser : SysUser
    {
        //0 为非会员
        public int MemberLevel { get; set; }

        public string MerchId { get; set; }
    }
}
