using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivVisitRecord")]
    public class SenvivVisitRecord
    {
        [Key]
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string VisitType { get; set; }
        public string RiskResult { get; set; }
        public string RiskFactor { get; set; }
        public string HealthAdvice { get; set; }
        public string Remark { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? NextTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
