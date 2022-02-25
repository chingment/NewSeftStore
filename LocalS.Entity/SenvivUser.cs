using System;
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
        Lady = 2,
        PrePregnancy = 24,
        Pregnancy = 25,
        Postpartum = 26
    }

    [Table("SenvivUser")]
    public class SenvivUser
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string UserId { get; set; }
        public string SvDeptId { get; set; }
        public string PhoneNumber { get; set; }
        public string NickName { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string Sex { get; set; }
        public DateTime? Birthday { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string SmTargetVal { get; set; }
        public DateTime? FisrtReportTime { get; set; }
        public string LastReportId { get; set; }
        public DateTime? LastReportTime { get; set; }
        public E_SenvivUserCareLevel CareLevel { get; set; }
        public E_SenvivUserCareMode CareMode { get; set; }
        //呼吸暂停综合证
        public string Sas { get; set; }
        //是否使用呼吸机
        public bool IsUseBreathMach { get; set; }
        //慢性病
        public string Chronicdisease { get; set; }
        //亚健康
        public string SubHealth { get; set; }
        public string SubHealthOt { get; set; }
        //目前困扰
        public string Perplex { get; set; }
        public string PerplexOt { get; set; }
        //既往史
        public string MedicalHis { get; set; }
        public string MedicalHisOt { get; set; }
        //用药情况
        public string Medicine { get; set; }
        public string MedicineOt { get; set; }
        //传染病
        public string Infection { get; set; }
        public string InfectionOt { get; set; }
        //过敏药
        public string Allergy { get; set; }
        public string AllergyOt { get; set; }

        public int DeviceCount { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
