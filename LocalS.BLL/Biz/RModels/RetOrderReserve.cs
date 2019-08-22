using LocalS.Entity;
using System.Collections.Generic;


namespace Lumos.BLL.Biz
{




    public class RetOrderReserve
    {
        public RetOrderReserve()
        {

        }
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
    }



    public class OrderReserveDetail
    {
        public OrderReserveDetail()
        {
            this.Details = new List<DetailChild>();
        }
        public E_StoreSellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public int Quantity { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public List<DetailChild> Details { get; set; }


        public class DetailChild
        {
            public DetailChild()
            {
                this.Details = new List<DetailChildSon>();
                this.SlotStock = new List<SlotStock>();
            }
            public E_StoreSellChannelRefType SellChannelRefType { get; set; }
            public string SellChannelRefId { get; set; }
            public string SkuId { get; set; }
            public string SkuName { get; set; }
            public string SkuImgUrl { get; set; }
            public decimal SalePrice { get; set; }

            public decimal SalePriceByVip { get; set; }
            public int Quantity { get; set; }
            public decimal OriginalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal ChargeAmount { get; set; }
            public List<DetailChildSon> Details { get; set; }

            public List<SlotStock> SlotStock { get; set; }
        }

        public class DetailChildSon
        {
            public string Id { get; set; }
            public E_StoreSellChannelRefType SellChannelRefType { get; set; }
            public string SellChannelRefId { get; set; }
            public string SlotId { get; set; }
            public string SkuId { get; set; }
            public int Quantity { get; set; }
            public string SkuName { get; set; }
            public string SkuImgUrl { get; set; }
            public decimal SalePrice { get; set; }
            public decimal SalePriceByVip { get; set; }
            public decimal OriginalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal ChargeAmount { get; set; }
        }

        public class SlotStock
        {
            public E_StoreSellChannelRefType SellChannelRefType { get; set; }
            public string SellChannelRefId { get; set; }
            public string SlotId { get; set; }
            public string SkuId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
