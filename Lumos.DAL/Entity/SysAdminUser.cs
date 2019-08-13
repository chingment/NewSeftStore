using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.DbRelay
{
    [Table("SysAdminUser")]
    public class SysAdminUser : SysUser
    {
        public bool IsMaster { get; set; }
    }
}
