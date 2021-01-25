using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("PayTransSub")]
    public class PayTransSub
    {
        [Key]
        public string Id { get; set; }
        public string PayTransId { get; set; }
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string ShopId { get; set; }
        public string ShopName { get; set; }
        public string OrderId { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public int Quantity { get; set; }
        public DateTime? SubmittedTime { get; set; }
        public DateTime? PayedTime { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string PayPartnerPayTransId { get; set; }
        public E_PayStatus PayStatus { get; set; }
        public E_PayWay PayWay { get; set; }
        public E_PayCaller PayCaller { get; set; }
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
