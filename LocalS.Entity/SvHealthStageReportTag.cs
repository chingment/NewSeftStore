using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SvHealthStageReportTag")]
    public class SvHealthStageReportTag
    {
        [Key]
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string ReportId { get; set; }
        public int TagId{ get; set; }
        public string TagName { get; set; }
        public int TagCount { get; set; }

    }
}
