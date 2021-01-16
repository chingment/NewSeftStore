using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Lumos.DbRelay
{
    [Table("SysUserOperateLog")]
    public class SysUserOperateLog
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AppId { get; set; }
        public string EventCode { get; set; }
        public string EventName { get; set; }
        public string EventData { get; set; }
        [MaxLength(1024)]
        public string Remark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
