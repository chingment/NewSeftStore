using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{

    public class RopCouponMy
    {
        public bool IsGetHis { get; set; }
        public List<string> CouponIds { get; set; }
        public E_ShopMethod ShopMethod { get; set; }
        public string StoreId { get; set; }
        public List<OrderConfirmProductSkuModel> ProductSkus { get; set; }
    }
}
