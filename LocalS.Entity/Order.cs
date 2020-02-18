using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    public enum E_OrderStatus
    {
        Unknow = 0,
        Submitted = 1000,
        WaitPay = 2000,
        Payed = 3000,
        Completed = 4000,
        Canceled = 5000
    }
    public enum E_OrderSource
    {
        Unknow = 0,
        Machine = 1,
        Api = 2,
        WechatMiniProgram = 3
    }

    public enum E_OrderPayWay
    {
        Unknow = 0,
        Wechat = 1,//微信支付
        AliPay = 2 //支付宝支付
    }

    public enum E_OrderPayPartner
    {
        Unknow = 0,
        Wx = 1,//微信支付
        Ali = 2, //支付宝支付
        Tg = 91, //通莞金服
        Xrt = 92 //通莞金服
    }

    public enum E_OrderPayCaller
    {
        Unknow = 0,
        WxByNt = 10, //微信方式生成二维码
        WxByPa = 11, //微信公众号发起支付
        WxByMp = 12, //微信小程序发起支付,
        AliByNt = 20, //支付宝方式生成二维码
        AggregatePayByNt = 90 //聚合方式生成二维码
    }


    public class PayOption
    {
        public E_OrderPayCaller Caller { get; set; }
        public E_OrderPayPartner Partner { get; set; }
        public List<E_OrderPayWay> SupportWays { get; set; }
    }

    [Table("Order")]
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string SellChannelRefIds { get; set; }
        public string CouponIds { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int Quantity { get; set; }
        public DateTime? SubmittedTime { get; set; }
        public DateTime? PayedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime? CanceledTime { get; set; }
        public E_OrderSource Source { get; set; }
        public E_OrderStatus Status { get; set; }
        public string CancelReason { get; set; }
        public string CancelOperator { get; set; }
        public E_OrderPayPartner PayPartner { get; set; }
        public E_OrderPayWay PayWay { get; set; }
        public E_OrderPayCaller PayCaller { get; set; }
        public string PayPrepayId { get; set; }
        public DateTime? PayExpireTime { get; set; }
        public string PayQrCodeUrl { get; set; }
        public string PickupCode { get; set; }
        public DateTime? PickupCodeExpireTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public bool ExIsHappen { get; set; }
        public DateTime? ExHappenTime { get; set; }
        public bool ExIsHandle { get; set; }
        public DateTime? ExHandleTime { get; set; }
    }
}
