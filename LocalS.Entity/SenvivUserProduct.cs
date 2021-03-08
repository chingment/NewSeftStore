using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    [Table("SenvivUserProduct")]
    public class SenvivUserProduct
    {
        [Key]
        public string Id { get; set; }
        public string DeptName { get; set; }
        public string Sn { get; set; }
        public string QrcodeUrl { get; set; }
        public string DeviceQRCode { get; set; }
        public string TcpAddress { get; set; }
        public string WebUrl { get; set; }
        public DateTime? LastOnlineTime { get; set; }
        public string Model { get; set; }
        public string SvUserId { get; set; }
        public DateTime? BindTime { get; set; }
        public string Status { get; set; }
        public string Version { get; set; }
        public string Remarks { get; set; }
        public string Batch { get; set; }
        public string Scmver { get; set; }
        public string Sovn { get; set; }
        public string Epromvn { get; set; }
        public string Imsi { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string DeptId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
