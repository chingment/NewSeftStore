using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("SenvivMerch")]
    public class SenvivMerch
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string WxPaAppName { get; set; }
        public string WxPaAppId { get; set; }
        public string WxPaAppSecret { get; set; }
        public string WxPaQrCode { get; set; }
        public string WxPaTplIdMonthReport { get; set; }
        public string WxPaTplIdDayReport { get; set; }
        public string WxPaTplIdHealthMonitor { get; set; }
        public string WxPaTplIdPregnancyRemind { get; set; }
        public string WxPaTplIdDeviceBindNotify { get; set; }
        public string WxPaTplIdDeviceUnBindNotify { get; set; }
        public string SmsSignName { get; set; }
        public string SmsTemplateCode { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
