using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupCouponMy
    {
        public bool IsGetHis { get; set; }
        public List<OrderConfirmProductModel> Products { get; set; }
        public List<string> CouponId { get; set; }
    }
}
