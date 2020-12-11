using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public enum E_ShopMethod
    {

        Unknow = 0,
        Shopping = 1,
        Hire = 2
    }

    public class RopOrderReserve
    {
        public RopOrderReserve()
        {
            this.Blocks = new List<OrderReserveBlockModel>();
        }

        public string AppId { get; set; }
        public string StoreId { get; set; }
        public string ClientUserId { get; set; }
        public string SvcAnswererId { get; set; }
        public string SaleOutletId { get; set; }
        public List<string> CouponIds { get; set; }
        public E_ShopMethod ShopMethod { get; set; }
        public E_OrderSource Source { get; set; }
        public List<OrderReserveBlockModel> Blocks { get; set; }
        public bool IsTestMode { get; set; }

    }
}
