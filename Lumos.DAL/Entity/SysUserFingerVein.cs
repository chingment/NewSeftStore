using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.DbRelay.Entity
{
    [Table("SysUserFingerVein")]
    public class SysUserFingerVein
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string MerchId { get; set; }
        public string VeinData { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
