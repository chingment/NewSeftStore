using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupProductDetails
    {
        public string SkuId { get; set; }

        public string StoreId { get; set; }

        public E_SellChannelRefType ShopMode { get; set; }

        public E_OrderShopMethod ShopMethod { get; set; }

        public string ShopId { get; set; }
    }
}
