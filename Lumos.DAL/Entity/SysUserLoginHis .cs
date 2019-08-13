using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.DbRelay
{
    [Table("SysUserLoginHis")]
    public class SysUserLoginHis
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public Enumeration.LoginWay LoginWay { get; set; }
        [MaxLength(128)]
        public string Ip { get; set; }
        [MaxLength(128)]
        public string Country { get; set; }
        [MaxLength(128)]
        public string Province { get; set; }
        [MaxLength(128)]
        public string City { get; set; }
        public string Location { get; set; }
        public DateTime LoginTime { get; set; }
        public Enumeration.LoginResult Result { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
