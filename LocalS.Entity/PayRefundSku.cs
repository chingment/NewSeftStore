using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("PayRefundSku")]
    public class PayRefundSku
    {
        public string Id { get; set; }
        public string PayRefundId { get; set; }
        public string UniqueId { get; set; }
        public bool SignRefunded { get; set; }
        public bool IsRefunded { get; set; }
        public decimal RefundedAmount { get; set; }
        public int RefundedQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
