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
        //public string SkuId { get; set; }
        //public string SkuCumCode { get; set; }
        //public string SkuMainImgUrl { get; set; }
        //public string SkuName { get; set; }
        public bool ApplySignRefunded { get; set; }
        public decimal ApplyRefundedAmount { get; set; }
        public int ApplyRefundedQuantity { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
