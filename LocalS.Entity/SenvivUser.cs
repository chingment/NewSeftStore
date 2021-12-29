﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_SenvivUserCareLevel
    {
        None = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4
    }

    public enum E_SenvivUserCareMode
    {
        None = 0,
        Normal = 1,
        Gravida = 2
    }

    [Table("SenvivUser")]
    public class SenvivUser
    {
        [Key]
        public string Id { get; set; }
        public string DeptId { get; set; }
        public string MerchId { get; set; }
        //public string Code { get; set; }
        public string WxOpenId { get; set; }
        public string PhoneNumber { get; set; }
        //public string Email { get; set; }
        //public string Pwd { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        //public string Language { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string TargetValue { get; set; }
        //public string Remarks { get; set; }
        //public DateTime? LastLoginTime { get; set; }
        //public string LoginCount { get; set; }

        public DateTime? FisrtReportTime { get; set; }
        public string LastReportId { get; set; }
        public DateTime? LastReportTime { get; set; }
        //public string Details { get; set; }
        public E_SenvivUserCareLevel CareLevel { get; set; }
        public E_SenvivUserCareMode CareMode { get; set; }
        //public string Status { get; set; }
        public string Sas { get; set; }
        public bool IsUseBreathMach{ get; set; }
        public string Perplex { get; set; }
        public string PerplexOt { get; set; }
        public string MedicalHis { get; set; }
        public string MedicalHisOt{ get; set; }
        public string Medicine { get; set; }
        public string MedicineOt { get; set; }
        public string Infection { get; set; }
        public string InfectionOt { get; set; }
        public string Allergy { get; set; }
        public string AllergyOt { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
