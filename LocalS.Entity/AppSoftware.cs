using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LocalS.Entity
{
    [Table("AppSoftware")]
    public class AppSoftware
    {
        [Key]
        public string Id { get; set; }
        public string AppId { get; set; }
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string VersionName { get; set; }
        public string VersionCode { get; set; }
        public string DownloadUrl { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
