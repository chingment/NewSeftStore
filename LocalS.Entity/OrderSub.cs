﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("OrderSub")]
    public class OrderSub
    {
        [Key]
        public string Id { get; set; }
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public E_ShopMode ShopMode { get; set; }
        public string MachineId { get; set; }
        public string MachineCumCode { get; set; }
        public string ReceiveModeName { get; set; }
        public E_ReceiveMode ReceiveMode { get; set; }
        public string OrderId { get; set; }
        public string SpuId { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public string SkuCumCode { get; set; }
        public string SkuBarCode { get; set; }
        public string SkuSpecDes { get; set; }
        public string SkuProducer { get; set; }
        public string SkuMainImgUrl { get; set; }
        public string SkuDeviceSn{ get; set; }
        public int KindId1 { get; set; }
        public int KindId2 { get; set; }
        public int KindId3 { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public E_RentTermUnit RentTermUnit { get; set; }
        public int RentTermValue { get; set; }
        public decimal RentAmount { get; set; }
        public decimal DepositAmount { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public E_PayStatus PayStatus { get; set; }
        public E_PayWay PayWay { get; set; }
        public DateTime? PayedTime { get; set; }
        public bool ExPickupIsHappen { get; set; }
        public DateTime? ExPickupHappenTime { get; set; }
        public string ExPickupReason { get; set; }
        public bool ExPickupIsHandle { get; set; }
        public DateTime? ExPickupHandleTime { get; set; }
        public E_OrderExPickupHandleSign ExPickupHandleSign { get; set; }
        public E_OrderPickupStatus PickupStatus { get; set; }
        public DateTime? PickupStartTime { get; set; }
        public DateTime? PickupEndTime { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public string PickupFlowLastDesc { get; set; }
        public DateTime? PickupFlowLastTime { get; set; }
        public string SvcConsulterId { get; set; }
        public bool IsTestMode { get; set; }
        public string SaleOutletId { get; set; }
        public E_ShopMethod ShopMethod { get; set; }
        public decimal CouponAmountByShop { get; set; }
        public decimal CouponAmountByRent { get; set; }
        public decimal CouponAmountByDeposit { get; set; }
        public string ReffSign { get; set; }
        public string ReffUserId { get; set; }
    }
}
