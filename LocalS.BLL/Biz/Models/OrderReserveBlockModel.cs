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
            this.Delivery = new DeliveryModel();
            this.SelfTake = new SelfTakeModel();
            this.Skus = new List<ProductSkuModel>();
            this.BookTime = new BookTimeModel();

        }
        public E_ReceiveMode ReceiveMode { get; set; }
        public E_SellChannelRefType ShopMode { get; set; }
        public DeliveryModel Delivery { get; set; }
        public SelfTakeModel SelfTake { get; set; }
        public BookTimeModel BookTime { get; set; }
        public List<ProductSkuModel> Skus { get; set; }
        public class DeliveryModel
        {
            public string Id { get; set; }
            public string Consignee { get; set; }
            public string PhoneNumber { get; set; }
            public string AreaName { get; set; }
            public string AreaCode { get; set; }
            public string Address { get; set; }
        }
        public class SelfTakeModel
        {
            public string Consignee { get; set; }
            public string PhoneNumber { get; set; }
            public string AreaName { get; set; }
            public string AreaCode { get; set; }
            public string StoreName { get; set; }
            public string StoreAddress { get; set; }

        }
        public class ProductSkuModel
        {
            public string CartId { get; set; }
            public string Id { get; set; }
            public int Quantity { get; set; }
            public E_SellChannelRefType ShopMode { get; set; }
            public string[] SellChannelRefIds { get; set; }

            public string SvcConsulterId { get; set; }
        }
        public class BookTimeModel
        {
            public string Value { get; set; }
            public int Type { get; set; }
        }
    }
}
