using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LocalS.Entity
{
    [Table("RentOrderTransRecord")]
    public class RentOrderTransRecord
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string OrdeId { get; set; }
        public string RentOrderId { get; set; }
        public string ClientUserId { get; set; }
        public E_RentTransTpye TransType { get; set; }
        public DateTime TransTime { get; set; }
        public E_RentAmountType AmountType { get; set; }
        public decimal Amount { get; set; }
        public DateTime? NextPayRentTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
