using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lumos.DbRelay
{
    [Table("SysAgentUser")]
    public class SysAgentUser : SysUser
    {
        public string AgentId { get; set; }
        public string TppId { get; set; }
        public bool IsMaster { get; set; }
    }
}
