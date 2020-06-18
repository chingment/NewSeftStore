using LocalS.Entity;
using Lumos;
using System.Collections.Generic;


namespace LocalS.BLL.Biz
{




    public class RetOrderReserve
    {
        public RetOrderReserve()
        {

        }
        public string OrderId { get; set; }
        public string ChargeAmount { get; set; }
    }

    public class BuildOrderSub
    {
        public BuildOrderSub()
        {
            this.Childs = new List<Child>();
        }
        public E_SellChannelRefType SellChannelRefType { get; set; }
        public string SellChannelRefId { get; set; }
        public int Quantity { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ChargeAmount { get; set; }
        public List<Child> Childs { get; set; }
        public class Child
        {
            public E_SellChannelRefType SellChannelRefType { get; set; }
            public string SellChannelRefId { get; set; }
            public string ProductSkuId { get; set; }
            public decimal SalePrice { get; set; }
            public decimal SalePriceByVip { get; set; }
            public int Quantity { get; set; }
            public decimal OriginalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal ChargeAmount { get; set; }
            public string CabinetId { get; set; }
            public string SlotId { get; set; }
        }
        public class ProductSku
        {
            public ProductSku()
            {
                this.Stocks = new List<ProductSkuStockModel>();
            }

            public string Id { get; set; }
            public int Quantity { get; set; }
            public E_SellChannelRefType ShopMode { get; set; }
            public List<ProductSkuStockModel> Stocks { get; set; }

            public string BarCode { get; set; }
            public string CumCode { get; set; }
            public string Producer { get; set; }
            public string PinYinIndex { get; set; }
            public string ProductId { get; set; }
            public string Name { get; set; }
            public string MainImgUrl { get; set; }
            public List<ImgSet> DisplayImgUrls { get; set; }
            public List<ImgSet> DetailsDes { get; set; }
            public List<SpecDes> SpecDes { get; set; }
            public string BriefDes { get; set; }
            public List<SpecItem> SpecItems { get; set; }
            public string SpecIdx { get; set; }

            public string CartId { get; set; }
        }

    }
}
