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
        public string OrderId { get; set; }
        public string StoreId { get; set; }
        public List<OrderConfirmProductModel> Products { get; set; }
        public List<string> CouponId { get; set; }

        public E_AppCaller Caller { get; set; }
    }
}
