using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivHealthMonthReportTag")]
    public class SenvivHealthMonthReportTag
    {
        [Key]
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string ReportId { get; set; }
        public string TagId{ get; set; }
        public string TagName { get; set; }
        public int TagCount { get; set; }

    }
}
