using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_RptOrderTradeType
    {
        Unknow = 0,
        Pay = 1,//支付单
        Refund = 2 //退款单
    }

    [Table("RptOrder")]
    public class RptOrder
    {
        [Key]
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string ClientUserId { get; set; }
        public E_RptOrderTradeType TradeType { get; set; }
        public decimal TradeAmount { get; set; }
        public int Quantity { get; set; }
        public DateTime TradeTime { get; set; }
    }
}
