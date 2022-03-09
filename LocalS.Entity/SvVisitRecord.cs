using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SvVisitRecordVisitType
    {
        None = 0,
        Callout = 1,
        WxPa = 2
    }

    public enum E_SvVisitRecordVisitTemplate
    {
        None = 0,
        CalloutRecord = 1,
        WxPaByHealthMonitor = 2
    }

    [Table("SvVisitRecord")]
    public class SvVisitRecord
    {
        [Key]
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public E_SvVisitRecordVisitType VisitType { get; set; }
        public E_SvVisitRecordVisitTemplate VisitTemplate { get; set; }
        public string VisitContent { get; set; }
        //public string RiskResult { get; set; }
        //public string RiskFactor { get; set; }
        //public string HealthAdvice { get; set; }
        public string ReportId { get; set; }
        public string TaskId { get; set; }
        public DateTime VisitTime { get; set; }
        public DateTime? NextTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
