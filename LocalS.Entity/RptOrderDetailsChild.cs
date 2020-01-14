using LocalS.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("RptOrderDetailsChild")]
    public class RptOrderDetailsChild
    {
        [Key]
        public string Id { get; set; }
        public string RptOrderId { get; set; }
        public string RptOrderDetailsId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
        public string ClientUserId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public string PrdProductSkuName { get; set; }
        public string PrdProductSkuCumCode { get; set; }
        public string PrdProductSkuBarCode { get; set; }
        public string PrdProductSkuSpecDes { get; set; }
        public string PrdProductSkuProducer { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public E_OrderPayWay PayWay { get; set; }
        public E_RptOrderTradeType TradeType { get; set; }
        public decimal TradeAmount { get; set; }
        public DateTime TradeTime { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefName { get; set; }
        public string SellChannelRefId { get; set; }
    }
}
