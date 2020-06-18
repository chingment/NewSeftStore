using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    public enum E_OrderPickupStatus
    {
        Unknow = 0,
        Submitted = 1000,
        WaitPay = 2000,
        Payed = 3000,
        WaitPickup = 3010,
        SendPickupCmd = 3011,
        Pickuping = 3012,
        Taked = 4000,
        Canceled = 5000,
        Exception = 6000,
        ExPickupSignTaked = 6010,
        ExPickupSignUnTaked = 6011
    }

    public enum E_OrderExPickupHandleSign
    {
        Unknow = 0,
        Taked = 1,
        UnTaked = 2
    }

    [Table("OrderSubChild")]
    public class OrderSubChild
    {
        [Key]
        public string Id { get; set; }
        public string ClientUserId { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public string SellChannelRefName { get; set; }
        public string OrderId { get; set; }
        public string OrderSubId { get; set; }
        public string PrdProductId { get; set; }
        public string PrdProductSkuId { get; set; }
        public string PrdProductSkuName { get; set; }
        public string PrdProductSkuCumCode { get; set; }
        public string PrdProductSkuBarCode { get; set; }
        public string PrdProductSkuSpecDes { get; set; }
        public string PrdProductSkuProducer { get; set; }
        public string PrdProductSkuMainImgUrl { get; set; }
        public int Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SalePriceByVip { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
        public E_OrderPayStatus PayStatus { get; set; }
        public E_OrderPayWay PayWay { get; set; }
        public DateTime? PayedTime { get; set; }

        public bool ExPickupIsHappen { get; set; }
        public DateTime? ExPickupHappenTime { get; set; }
        public bool ExPickupIsHandle { get; set; }
        public DateTime? ExPickupHandleTime { get; set; }
        public E_OrderExPickupHandleSign ExPickupHandleSign { get; set; }
        public E_OrderPickupStatus PickupStatus { get; set; }
        public DateTime? PickupStartTime { get; set; }
        public DateTime? PickupEndTime { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }
        public int LastPickupActionId { get; set; }
        public int LastPickupActionStatusCode { get; set; }
    }
}
