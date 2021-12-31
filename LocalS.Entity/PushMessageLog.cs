using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{

    [Table("PushMessageLog")]
    public class PushMessageLog
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string CdType { get; set; }
        public string CdValue { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
