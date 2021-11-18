using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_StoreStatus
    {

        Unknow = 0,
        Valid = 1,
        Invalid = 2
    }

    [Table("Store")]
    public class Store
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactAddress { get; set; }
        public bool IsDelete { get; set; }
        public string BriefDes { get; set; }
        public bool IsTestMode { get; set; }
        public string WxPayMchId { get; set; }
        public string WxPayKey { get; set; }
        public string WxPayResultNotifyUrl { get; set; }
        public string WxPayCertPath { get; set; }
        public string WxPayCertPassword { get; set; }
        public string WxPaNotifyEventUrlToken { get; set; }
        public string WxPaOauth2RedirectUrl { get; set; }
        public string WxPaAppId { get; set; }
        public string WxPaAppSecret { get; set; }
        public string WxMpAppId { get; set; }
        public string WxMpAppSecret { get; set; }
        public string ZfbMpAppId { get; set; }
        public string ZfbMpAppPrivateSecret { get; set; }
        public string ZfbPublicSecret { get; set; }
        public string ZfbResultNotifyUrl { get; set; }
        public string TgPayAccount { get; set; }
        public string TgPayKey { get; set; }
        public string TgPayResultNotifyUrl { get; set; }
        public string XrtPayMchId { get; set; }
        public string XrtPayKey { get; set; }
        public string XrtPayResultNotifyUrl { get; set; }
        /// <summary>
        /// 终端设备，构建生成二维码的选项，1：，微信，2：支付宝，3：银联，4：京东钱包，5：掌上生活，9：聚合支付，当不是使用聚合支付 99：聚合一码 不显示
        /// </summary>
        public string TermAppPayOptions { get; set; }
        public string WxmpAppPayOptions { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
