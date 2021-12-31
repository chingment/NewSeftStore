using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivMerchConfig")]
    public class SenvivMerchConfig
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string DeptId { get; set; }
        public string TplIdMonthReport { get; set; }
        public string TplIdHealthMonitor { get; set; }
        public string TplIdPregnancyRemind { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
