using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SvHealthDayReportAdvice")]
    public class SvHealthDayReportAdvice
    {
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string ReportId { get; set; }
        public string SuggestCode { get; set; }
        public string SuggestName { get; set; }
        public string Summary{ get; set; }
        public string SuggestDirection { get; set; }
    }
}
