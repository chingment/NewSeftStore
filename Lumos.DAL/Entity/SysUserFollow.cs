using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.DbRelay
{
    [Table("SysUserFollow")]
    public class SysUserFollow
    {
        public string Id { get; set; }
        [Key, Column(Order = 1)]
        public string UserId { get; set; }
        [Key, Column(Order = 2)]
        public string FollowUserId { get; set; }
        public bool IsDelete { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
