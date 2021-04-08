using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{

    public enum E_SenvivHealthMonthStatus
    {
        Unknow = 0,
        WaitBuild = 1,
        Building = 2,
        BuildSuccess = 3,
        BuildFailure = 4,
        WaitSend = 5,
        Sending = 6,
        SendSuccess = 7,
        SendFailure = 8
    }

    [Table("SenvivHealthMonthReport")]
    public class SenvivHealthMonthReport
    {
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string HealthDate { get; set; }
        public int DayCount { get; set; }
        public decimal TotalScore { get; set; }
        public decimal SmSmsc { get; set; }
        public string SmSmscPt { get; set; }
        public decimal SmQdsmsc { get; set; }
        public decimal SmSdsmsc { get; set; }
        public decimal SmRemsmsc { get; set; }
        public decimal SmDtqcs { get; set; }
        public string SmDtqcsPt { get; set; }
        public decimal HrvXzznl { get; set; }
        public string HrvXzznlPt { get; set; }
        public decimal HxPjhx { get; set; }
        public decimal XlPjxl { get; set; }
        public decimal HxZtpjahizs { get; set; }
        public decimal HxZtcs { get; set; }
        public string HxZtcsPt { get; set; }
        public decimal SmTdcs { get; set; }

        public string SmTags { get; set; }
        public bool IsBuild { get; set; }
        public bool IsSend { get; set; }
        public int VisitCount { get; set; }
        public E_SenvivHealthMonthStatus Status { get; set; }
        public string SugByYd { get; set; }
        public string SugByYy { get; set; }
        public string SugBySm { get; set; }
        public string SugByQxyl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
