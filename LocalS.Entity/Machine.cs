using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_MachineRunStatus
    {
        Unknow = 0,
        Running = 1,
        Stoped = 2,
        Setting = 3
    }

    [Table("Machine")]
    public class Machine
    {
        [Key]
        public string Id { get; set; }
        [MaxLength(128)]
        public string Name { get; set; }
        public string ImeiId { get; set; }
        public string DeviceId { get; set; }
        public string MainImgUrl { get; set; }
        [MaxLength(128)]
        public string MacAddress { get; set; }
        public float Lat { get; set; }
        public float Lng { get; set; }
        public string JPushRegId { get; set; }
        public string CurUseMerchId { get; set; }
        public string CurUseStoreId { get; set; }
        public string LogoImgUrl { get; set; }
        public string AppVersionCode { get; set; }
        public string AppVersionName { get; set; }
        public DateTime? LastRequestTime { get; set; }
        public E_MachineRunStatus RunStatus { get; set; }
        public string CtrlSdkVersionCode { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public bool IsTestMode { get; set; }
        public bool KindIsHidden { get; set; }
        public int KindRowCellSize { get; set; }
        public bool CameraByChkIsUse { get; set; }
        public bool CameraByJgIsUse { get; set; }
        public bool CameraByRlIsUse { get; set; }
        public bool SannerIsUse { get; set; }
        public string SannerComId { get; set; }
        public bool FingerVeinnerIsUse { get; set; }
        public bool ExIsHas { get; set; }
        public string MstVern { get; set; } //门所在主控商
        public string OstVern { get; set; } //系统在主控商

        public bool ImIsUse { get; set; }
        public string ImPartner { get; set; }
        public string ImUserName { get; set; }
        public string ImPassword { get; set; }
    }
}
