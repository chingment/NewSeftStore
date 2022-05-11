using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.DbRelay
{
    [Table("SysUserContact")]
    public class SysUserContact
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public int Type { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsEnable { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
