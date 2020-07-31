using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RopOrderConfirm
    {
        public List<Order> Orders { get; set; }
        public string StoreId { get; set; }
        public List<OrderConfirmProductSkuModel> ProductSkus { get; set; }
        public List<string> CouponId { get; set; }
        public E_AppCaller Caller { get; set; }

        public class Order
        {
            public string Id { get; set; }
        }
    }
}
