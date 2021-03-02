using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_RentTermUnit
    {
        Unknow = 0,
        Year = 1,
        Quarter = 2,
        Month = 3,
        Day = 4,
        Hour = 5,
        Minute = 6
    }
    public enum E_RentTransTpye
    {
        Unknow = 0,
        Pay = 1,
        Refund = 2
    }

    public enum E_RentAmountType
    {
        Unknow = 0,
        DepositAndRent = 1,
        Deposit = 2,
        Rent = 3
    }


    [Table("RentOrder")]
    public class RentOrder
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string OrdeId { get; set; }
        public string ClientUserId { get; set; }
        public string SpuId { get; set; }
        public string SkuId { get; set; }
        public string SkuName { get; set; }
        public string SkuCumCode { get; set; }
        public string SkuBarCode { get; set; }
        public string SkuSpecDes { get; set; }
        public string SkuProducer { get; set; }
        public string SkuMainImgUrl { get; set; }
        public string SkuDeviceSn { get; set; }
        public decimal DepositAmount { get; set; }
        public bool IsPayDeposit { get; set; }
        public DateTime? PayDepositTime { get; set; }
        public bool IsReturn { get; set; }
        public decimal RefundAmount { get; set; }
        public DateTime? RefundTime { get; set; }
        public E_RentTermUnit RentTermUnit { get; set; }
        public int RentTermValue { get; set; }
        public string RentTermUnitText { get; set; }
        public decimal RentAmount { get; set; }
        public DateTime? NextPayRentTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
