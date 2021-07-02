using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("DeviceMqttMessage")]
    public class DeviceMqttMessage
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string DeviceId { get; set; }
        public string Method { get; set; }
        public string Params { get; set; }
        public string Version { get; set; }
        public bool IsArried { get; set; }
        public bool IsExecStart { get; set; }
        public bool IsExecEnd { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
