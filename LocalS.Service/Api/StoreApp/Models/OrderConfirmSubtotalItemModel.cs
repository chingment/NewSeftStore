using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class OrderBlockModel
    {
        public OrderBlockModel()
        {
            this.Skus = new List<OrderConfirmProductSkuModel>();
            this.Delivery = new DeliveryModel();
            this.SelfTake = new SelfTakeModel();
        }

        public string TagName { get; set; }
        public E_SellChannelRefType ShopMode { get; set; }
        public DeliveryModel Delivery { get; set; }
        public SelfTakeModel SelfTake { get; set; }
        public List<OrderConfirmProductSkuModel> Skus { get; set; }
        public E_TabMode TabMode { get; set; }
    }

    public enum E_TabMode
    {

        Unknow = 0,
        Delivery = 1,
        SelfTake = 2,
        DeliveryAndSelfTake = 3
    }

    public class OrderConfirmSubtotalItemModel
    {
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public bool IsDcrease { get; set; }
    }
}
