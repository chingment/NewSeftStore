using LocalS.Entity;
using System.Collections.Generic;


namespace LocalS.BLL.Biz
{




    public class RetOrderReserve
    {
        public RetOrderReserve()
        {

        }
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
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

        public E_ReceptionMode ReceptionMode { get; set; }
        public class Child
        {
            public Child()
            {
                this.Units = new List<Unit>();
                this.SlotStock = new List<SlotStock>();
            }
            public E_SellChannelRefType SellChannelRefType { get; set; }
            public string SellChannelRefId { get; set; }

            public string ProductId { get; set; }
            public string ProductSkuId { get; set; }
            public decimal SalePrice { get; set; }
            public decimal SalePriceByVip { get; set; }
            public int Quantity { get; set; }
            public decimal OriginalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal ChargeAmount { get; set; }
            public List<Unit> Units { get; set; }

            public List<SlotStock> SlotStock { get; set; }
        }

        public class Unit
        {
            public string Id { get; set; }
            public E_SellChannelRefType SellChannelRefType { get; set; }
            public string SellChannelRefId { get; set; }
            public string SlotId { get; set; }
            public int Quantity { get; set; }
            public string ProductId { get; set; }
            public string ProductSkuId { get; set; }
            public decimal SalePrice { get; set; }
            public decimal SalePriceByVip { get; set; }
            public decimal OriginalAmount { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal ChargeAmount { get; set; }

            public E_ReceptionMode ReceptionMode { get; set; }
        }

        public class SlotStock
        {
            public E_SellChannelRefType SellChannelRefType { get; set; }
            public string SellChannelRefId { get; set; }
            public string SlotId { get; set; }
            public string ProductSkuId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
