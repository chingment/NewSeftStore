using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("BI_AppTraceLog")]

    public class BI_AppTraceLog
    {
        [Key]
        public string Id { get; set; }
        public string AppId { get; set; }
        public string Page { get; set; }
        public string Action { get; set; }
        public string Param { get; set; }
        public string UsrSign { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
