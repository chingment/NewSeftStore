using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class OrderReserveBlockModel
    {
        public OrderReserveBlockModel()
        {
            this.DeliveryAddress = new DeliveryAddressModel();
            this.Skus = new List<ProductSkuModel>();

        }
        public E_SellChannelRefType ShopMode { get; set; }

        public DeliveryAddressModel DeliveryAddress { get; set; }

        public List<ProductSkuModel> Skus { get; set; }

        public class DeliveryAddressModel
        {
            public string Id { get; set; }
            public string Consignee { get; set; }
            public string PhoneNumber { get; set; }
            public string AreaName { get; set; }
            public string AreaCode { get; set; }
            public string Address { get; set; }
        }

        public class ProductSkuModel
        {
            public string CartId { get; set; }
            public string Id { get; set; }
            public int Quantity { get; set; }
            public E_SellChannelRefType ShopMode { get; set; }
            public string[] SellChannelRefIds { get; set; }
        }
    }
}
