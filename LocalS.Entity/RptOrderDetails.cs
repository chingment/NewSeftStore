using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{

    [Table("RptOrderDetails")]
    public class RptOrderDetails
    {
        [Key]
        public string Id { get; set; }
        public string RptOrderId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string OrderId { get; set; }
        public string ClientUserId { get; set; }
        public int Quantity { get; set; }
        public E_RptOrderTradeType TradeType { get; set; }
        public decimal TradeAmount { get; set; }
        public DateTime TradeTime { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefName { get; set; }
        public string SellChannelRefId { get; set; }
    }
}
