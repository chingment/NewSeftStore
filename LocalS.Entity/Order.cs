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
        Cancled = 5000

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
        Wechat = 1,//微信支付
        AliPay = 2, //支付宝支付
        TongGuan = 91 //通莞金服
    }

    public enum E_OrderPayCaller
    {
        Unknow = 0,
        WechatByBuildQrCode = 10, //微信方式生成二维码
        WechatByPa = 11, //微信公众号发起支付
        WechatByMp = 12, //微信小程序发起支付,
        AlipayByBuildQrCode = 20, //支付宝方式生成二维码
        AggregatePayByBuildQrCode = 90 //聚合方式生成二维码

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
        public string AppId { get; set; }
        public string Sn { get; set; }
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int Quantity { get; set; }
        public string CouponId { get; set; }
        public DateTime? SubmitTime { get; set; }
        public DateTime? PayTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime? CancledTime { get; set; }
        public E_OrderSource Source { get; set; }
        public E_OrderStatus Status { get; set; }
        public string CancelReason { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public E_OrderPayPartner PayPartner { get; set; }
        public E_OrderPayWay PayWay { get; set; }
        public E_OrderPayCaller PayCaller { get; set; }
        public string PayPrepayId { get; set; }
        public DateTime? PayExpireTime { get; set; }
        public string PayQrCodeUrl { get; set; }
        public string PickCode { get; set; }
        public DateTime? PickCodeExpireTime { get; set; }
        public string SellChannelRefIds { get; set; }

    }
}
