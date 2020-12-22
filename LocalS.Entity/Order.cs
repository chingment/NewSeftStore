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
        Wxmp = 3
    }

    public enum E_OrderCancleType
    {
        Unknow = 0,
        PayCancle = 1,
        PayTimeout = 2
    }

    public enum E_OrderPickupStatus
    {
        Unknow = 0,
        Submitted = 1000,
        WaitPay = 2000,
        Payed = 3000,
        WaitPickup = 3010,
        SendPickupCmd = 3011,
        Pickuping = 3012,
        Taked = 4000,
        Canceled = 5000,
        Exception = 6000,
        ExPickupSignTaked = 6010,
        ExPickupSignUnTaked = 6011
    }

    public enum E_OrderExPickupHandleSign
    {
        Unknow = 0,
        Taked = 1,
        UnTaked = 2
    }

    public enum E_OrderShopMethod
    {

        Unknow = 0,
        Shop = 1,
        Rent = 2,
        MemberFee = 3
    }

    [Table("Order")]
    public class Order
    {
        [Key]
        public string Id { get; set; }
        public string ClientUserId { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public E_ReceiveMode ReceiveMode { get; set; }
        public string ReceiveModeName { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ReceptionAddress { get; set; }
        public string ReceptionAreaCode { get; set; }
        public string ReceptionAreaName { get; set; }
        public string ReceptionMarkName { get; set; }
        public DateTime? ReceptionBookTime { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        // public decimal CouponAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int Quantity { get; set; }
        public string PickupCode { get; set; }
        public DateTime? PickupCodeExpireTime { get; set; }
        /// <summary>
        /// 是否触发过取货
        /// </summary>
        public bool PickupIsTrg { get; set; }
        public DateTime? PickupTrgTime { get; set; }
        public E_PayStatus PayStatus { get; set; }
        public E_PayWay PayWay { get; set; }
        public DateTime? PayedTime { get; set; }
        public bool ExIsHappen { get; set; }
        public DateTime? ExHappenTime { get; set; }
        public bool ExIsHandle { get; set; }
        public string ExImgUrls { get; set; }
        public DateTime? ExHandleTime { get; set; }
        public string ExHandleRemark { get; set; }
        public string PickupFlowLastDesc { get; set; }
        public DateTime? PickupFlowLastTime { get; set; }
        public string ExpressNumber { get; set; }
        public string ExpressComName { get; set; }
        public string ExpressComId { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public DateTime? SubmittedTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public DateTime? CanceledTime { get; set; }
        public E_OrderSource Source { get; set; }
        public E_OrderStatus Status { get; set; }
        public string ClientUserName { get; set; }
        public string AppId { get; set; }
        public bool IsTestMode { get; set; }
        public DateTime? PayExpireTime { get; set; }
        public string CancelOperator { get; set; }
        public string CancelReason { get; set; }
        public decimal RefundedAmount { get; set; }
        public string PayTransId { get; set; }
        public string PayPartnerPayTransId { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public DateTime? BookStartTime { get; set; }
        public DateTime? BookEndTime { get; set; }
        public bool IsNoDisplayClient { get; set; }
        public string SaleOutletId { get; set; }
        public string CouponIdsByShop { get; set; }
        public decimal CouponAmountByShop { get; set; }
        public string CouponIdByRent { get; set; }
        public decimal CouponAmountByRent { get; set; }
        public string CouponIdByDeposit { get; set; }
        public decimal CouponAmountByDeposit { get; set; }
        public E_OrderShopMethod ShopMethod { get; set; }
    }
}
