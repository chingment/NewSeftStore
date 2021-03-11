using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivHealthDay")]
    public class SenvivHealthDay
    {
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string HealthDate { get; set; }
        public decimal Score { get; set; }
        public decimal XlAvg { get; set; }
        public decimal XlStd { get; set; }
        public decimal XlMin { get; set; }
        public decimal XlMax { get; set; }
        public decimal HxAvg { get; set; }
        public decimal HxStd { get; set; }
        public decimal HxMin { get; set; }
        public decimal HxMax { get; set; }
        public decimal HxTd { get; set; }
        public decimal HxZtNum { get; set; }
        public decimal HxZtStd { get; set; }
        public decimal HrvXzznl { get; set; }
        public decimal HrvXljsl { get; set; }
        public decimal HrvMzsjzl { get; set; }
        public decimal HrvJgsjzl { get; set; }
        public decimal HrvZzsjzl { get; set; }
        public decimal HrvXgss { get; set; }
        public decimal HrvHerm { get; set; }
        public decimal HrvSDNN { get; set; }
        public decimal HrvPNN50 { get; set; }
        public decimal HrvRMSSD { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
