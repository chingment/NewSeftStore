using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetOrderConfirm
    {
        public RetOrderConfirm()
        {
            this.SubtotalItems = new List<OrderConfirmSubtotalItemModel>();
            this.Blocks = new List<OrderBlockModel>();
        }

        //购物优惠卷
        public OrderConfirmCouponModel CouponByShop { get; set; }
        //租金优惠券
        public OrderConfirmCouponModel CouponByRent { get; set; }
        //押金优惠券
        public OrderConfirmCouponModel CouponByDeposit { get; set; }
        //订单块
        public List<OrderBlockModel> Blocks { get; set; }
        //小计项目
        public List<OrderConfirmSubtotalItemModel> SubtotalItems { get; set; }
        //实际支付金额
        public string ActualAmount { get; set; }
        //原金额
        public string OriginalAmount { get; set; }

        public List<string> OrderIds { get; set; }

    }
}
