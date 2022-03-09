using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_SvUserDeviceBindStatus
    {
        Unknow = 0,
        NotBind = 1,
        Binded = 2,
        UnBind = 3
    }

    [Table("SvUserDevice")]
    public class SvUserDevice
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string SvUserId { get; set; }
        public string SvDeptId { get; set; }
        public string DeviceId { get; set; }
        public DateTime? BindTime { get; set; }
        public DateTime? BindDeviceIdTime { get; set; }
        public DateTime? BindPhoneTime { get; set; }
        public DateTime? InfoFillTime { get; set; }
        public DateTime? UnBindTime { get; set; }
        public E_SvUserDeviceBindStatus BindStatus { get; set; }

        public string TcpAddress { get; set; }

        public string WebUrl { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
