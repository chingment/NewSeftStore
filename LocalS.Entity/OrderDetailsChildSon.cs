using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{

    public enum E_OrderDetailsChildSonStatus
    {
        Unknow = 0,
        Submitted = 1000,
        WaitPay = 2000,
        //Payed = 3000,
        WaitPick = 3010,
        SendPick = 3011,
        Picking = 3012,
        Completed = 4000,
        Cancled = 5000,
        Exception = 6000
    }

    [Table("OrderDetailsChildSon")]
    public class OrderDetailsChildSon
    {
        [Key]
        public string Id { get; set; }
        public string Sn { get; set; }
        public string ClientUserId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public string SellChannelRefName { get; set; }
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
        public string OrderDetailsId { get; set; }
        public string OrderDetailsSn { get; set; }
        public string OrderDetailsChildId { get; set; }
        public string OrderDetailsChildSn { get; set; }
        public string SlotId { get; set; }

        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public string PrdProductSkuName { get; set; }
        public string PrdProductSkuMainImgUrl { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SalePriceByVip { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public DateTime? PayTime { get; set; }
        public DateTime? SubmitTime { get; set; }
        public DateTime? CancledTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public E_OrderDetailsChildSonStatus Status { get; set; }
        public int LastPickupActionId { get; set; }
        public int LastPickupActionStatusCode { get; set; }

    }
}
