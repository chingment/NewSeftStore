﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivUser")]
    public class SenvivUser
    {
        [Key]
        public string Id { get; set; }
        public string DeptId { get; set; }
        public string Code { get; set; }
        public string WechatId { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string Nick { get; set; }
        public string Account { get; set; }
        public string HeadImgurl { get; set; }
        public string Language { get; set; }
        public string Sex { get; set; }
        public string Birthday { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string TargetValue { get; set; }
        public string Remarks { get; set; }
        public string LastloginTime { get; set; }
        public string LoginCount { get; set; }
        public string LastReportId { get; set; }
        public string LastReportTime { get; set; }
        public string Details { get; set; }
        public string Status { get; set; }
        public string SAS { get; set; }
        public string BreathingMachine { get; set; }
        public string Perplex { get; set; }
        public string OtherPerplex { get; set; }
        public string Medicalhistory { get; set; }
        public string OtherFamilyhistory { get; set; }
        public string Medicine { get; set; }
        public string OtherMedicine { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}