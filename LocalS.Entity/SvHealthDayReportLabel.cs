using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    [Table("SvHealthDayReportLabel")]
    public class SvHealthDayReportLabel
    {
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string ReportId { get; set; }
        public string TypeClass { get; set; }
        public decimal Score { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public string Explain { get; set; }
        public string Suggest { get; set; }
        public int Level { get; set; }
    }
}
