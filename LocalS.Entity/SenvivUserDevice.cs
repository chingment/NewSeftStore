using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum SenvivUserDeviceBindStatus
    {
        None = 0,
        Bind = 1,
        UnBind = 2
    }

    [Table("SenvivUserDevice")]
    public class SenvivUserDevice
    {
        [Key]
        public string Id { get; set; }
        public string SvUserId { get; set; }
        public string DeviceId { get; set; }
        public DateTime? BindTime { get; set; }
        public DateTime? BindDeviceIdTime{ get; set; }
        public DateTime? BindPhoneTime { get; set; }
        public DateTime? InfoFillTime { get; set; }
        public DateTime? UnBindTime { get; set; }
        public SenvivUserDeviceBindStatus BindStatus { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
