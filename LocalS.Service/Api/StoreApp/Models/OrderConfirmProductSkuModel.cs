using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class OrderConfirmProductSkuModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public decimal SalePrice { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SaleAmount { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CouponAmount { get; set; }
        public string BriefDes { get; set; }
        public string SpecDes { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public E_SellChannelRefType ShopMode { get; set; }

        public int Kind3 { get; set; }
    }
}

