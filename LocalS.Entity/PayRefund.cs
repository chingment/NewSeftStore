using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_PayRefundStatus
    {
        Unknow = 0,
        Applying = 1,
        Success = 2,
        Failure = 3
    }

    public enum E_PayRefundMethod
    {
        Unknow = 0,
        Original = 1,
        Manual = 2
    }
    
    [Table("PayRefund")]
    public class PayRefund
    {
        [Key]
        public string Id { get; set; }
        public string ClientUserId { get; set; }
        public string ClientUserName { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string OrderId { get; set; }
        public string PayTransId { get; set; }
        public string PayPartnerOrderId { get; set; }
        public DateTime? RefundTime { get; set; }
        public E_PayRefundMethod Method { get; set; }
        public decimal Amount { get; set; }
        public string Operator { get; set; }
        public string Reason { get; set; }
        public E_PayRefundStatus Status { get; set; }
        public DateTime? ApplyTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
