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
        Canceled = 5000,
        Exception = 6000,
        ExPickupSignTaked = 6010,
        ExPickupSignUnTaked = 6011
    }

    public enum E_OrderDetailsChildSonExPickupHandleSign
    {
        Unknow = 0,
        Taked = 1,
        UnTaked = 2
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
        public int LastPickupActionId { get; set; }
        public int LastPickupActionStatusCode { get; set; }
        public E_OrderDetailsChildSonStatus Status { get; set; }
        public bool ExPickupIsHandled { get; set; }
        public E_OrderDetailsChildSonExPickupHandleSign ExPickupHandleSign { get; set; }
        public string ExPickupHandleRemark { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
