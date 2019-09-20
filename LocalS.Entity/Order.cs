using System;
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
        Wechat = 1,
        AliPay = 2
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

        public E_OrderPayWay PayWay { get; set; }
        public string PayPrepayId { get; set; }
        public DateTime? PayExpireTime { get; set; }
        public string PayQrCodeUrl { get; set; }
        public string PickCode { get; set; }
        public DateTime? PickCodeExpireTime { get; set; }

    }
}
