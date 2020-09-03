using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_PayRefundStatus
    {
        Unknow = 0,
        WaitHandle = 1,
        Handling = 2,
        Success = 3,
        Failure = 4,
        InVaild = 5
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
        public string PayPartnerPayTransId { get; set; }


        public E_PayRefundStatus Status { get; set; }
        public E_PayRefundMethod ApplyMethod { get; set; }
        public decimal ApplyAmount { get; set; }
        public DateTime? ApplyTime { get; set; }
        public string ApplyRemark { get; set; }
        public string Applyer { get; set; }
        public DateTime? HandleTime { get; set; }
        public string HandleRemark { get; set; }
        public string Handler { get; set; }
        public DateTime? RefundedTime { get; set; }
        public decimal RefundedAmount { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
