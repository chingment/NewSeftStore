using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RopOrderReserve
    {
        public RopOrderReserve()
        {
            this.Blocks = new List<LocalS.BLL.Biz.RopOrderReserve.BlockModel>();
        }
        public string StoreId { get; set; }
        public string SaleOutletId { get; set; }
        public List<LocalS.BLL.Biz.RopOrderReserve.BlockModel> Blocks { get; set; }
        public E_OrderSource Source { get; set; }
        public E_ShopMethod ShopMethod { get; set; }
        public List<string> CouponIdsByShop { get; set; }
        public string CouponIdByRent { get; set; }
        public string CouponIdByDeposit { get; set; }
        public string ReffSign { get; set; }
    }
}
