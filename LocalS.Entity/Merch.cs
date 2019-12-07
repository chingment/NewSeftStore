using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("Merch")]
    public class Merch
    {
        [Key]
        public string Id { get; set; }
        public string MerchUserId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public string WxPayMchId { get; set; }
        public string WxPayKey { get; set; }
        public string WxPayResultNotifyUrl { get; set; }
        public string WxPaNotifyEventUrlToken { get; set; }
        public string WxPaOauth2RedirectUrl { get; set; }
        public string WxPaAppId { get; set; }
        public string WxPaAppSecret { get; set; }
        public string WxMpAppId { get; set; }
        public string WxMpAppSecret { get; set; }
        public string AlipayMpAppId { get; set; }
        public string AlipayMpAppPrivateSecret { get; set; }
        public string AlipayPublicSecret { get; set; }
        public string AlipayResultNotifyUrl { get; set; }
        public string TongGuanPayAccount { get; set; }
        public string TongGuanPayKey { get; set; }
        public string TongGuanPayResultNotifyUrl { get; set; }
        public string SupportPayPartner { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }

        public string CsrQrCode { get; set; }
    }
}
