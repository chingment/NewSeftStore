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
        }

        public string TagName { get; set; }
        public E_SellChannelRefType ReceptionMode { get; set; }
        public DeliveryAddressModel DeliveryAddress { get; set; }
        public List<OrderConfirmProductSkuModel> Skus { get; set; }
    }

    public class OrderConfirmSubtotalItemModel
    {
        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string Amount { get; set; }
        public bool IsDcrease { get; set; }
    }
}
