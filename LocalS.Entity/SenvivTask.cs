using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SenvivTaskType
    {
        None = 0,
        FisrtDay = 1,
        SevenDay = 2,
        FourteenDay = 3,
        PerMonth = 4
    }

    [Table("SenvivTask")]
    public class SenvivTask
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public E_SenvivTaskType TaskType { get; set; }
        public string Title { get; set; }
        public string HandleTime { get; set; }
        public string Handler { get; set; }
        public string ReportId { get; set; }
        public string Status { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
