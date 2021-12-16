using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivHealthStageReportSugSku")]
    public class SenvivHealthStageReportSugSku
    {
        [Key]
        public string Id { get; set; }
        public string ReportId { get; set; }
        public string MerchId { get; set; }
        public string SkuId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
