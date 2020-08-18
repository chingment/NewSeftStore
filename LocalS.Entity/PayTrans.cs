using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{

    public enum E_PayWay
    {
        Unknow = 0,
        Wx = 1,//微信支付
        Zfb = 2 //支付宝支付
    }

    public enum E_PayPartner
    {
        Unknow = 0,
        Wx = 1,//微信支付
        Zfb = 2, //支付宝支付
        Tg = 91, //通莞金服
        Xrt = 92 //深银联金服
    }

    public enum E_PayStatus
    {
        Unknow = 0,
        WaitPay = 1,
        Paying = 2,
        PaySuccess = 3,
        PayCancle = 4,
        PayTimeout = 5
    }

    public enum E_PayCaller
    {
        Unknow = 0,
        WxByNt = 10, //微信方式生成二维码
        WxByPa = 11, //微信公众号发起支付
        WxByMp = 12, //微信小程序发起支付,
        ZfbByNt = 20, //支付宝方式生成二维码
        AggregatePayByNt = 90 //聚合方式生成二维码
    }

    public class PayOption
    {
        public E_PayCaller Caller { get; set; }
        public E_PayPartner Partner { get; set; }
        public List<E_PayWay> SupportWays { get; set; }
    }


    [Table("PayTrans")]
    public class PayTrans
    {
        [Key]
        public string Id { get; set; }
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string OrderIds { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int Quantity { get; set; }
        public DateTime? SubmittedTime { get; set; }
        public DateTime? PayedTime { get; set; }
        public DateTime? CanceledTime { get; set; }
        public string CancelReason { get; set; }
        public string CancelOperator { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string PayPartnerPayTransId { get; set; }
        public E_PayStatus PayStatus { get; set; }
        public E_PayWay PayWay { get; set; }
        public E_PayCaller PayCaller { get; set; }
        public DateTime? PayExpireTime { get; set; }
        public E_OrderSource Source { get; set; }
        public bool IsTestMode { get; set; }
        public string AppId { get; set; }
        public string Description { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
