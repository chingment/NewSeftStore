using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("OrderSub")]
    public class OrderSub
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string ClientUserId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public string SellChannelRefName { get; set; }
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
        public string PickupCode { get; set; }
        public DateTime? PickupCodeExpireTime { get; set; }
        public string Receiver { get; set; }
        public string ReceiverPhone { get; set; }
        public string ReceptionAddress { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int Quantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        /// <summary>
        /// 是否触发过取货
        /// </summary>
        public bool PickupIsTrg { get; set; }
        public DateTime? PickupTrgTime { get; set; }
        public E_OrderPayStatus PayStatus { get; set; }
        public E_OrderPayWay PayWay { get; set; }
        public DateTime? PayedTime { get; set; }

        public bool ExIsHappen { get; set; }
        public DateTime? ExHappenTime { get; set; }
        public bool ExIsHandle { get; set; }
        public DateTime? ExHandleTime { get; set; }
    }
}
