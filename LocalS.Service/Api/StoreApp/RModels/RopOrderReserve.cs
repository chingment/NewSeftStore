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
            this.Blocks = new List<OrderReserveBlockModel>();
        }

        public string StoreId { get; set; }

        public string SaleOutletId { get; set; }
        public List<OrderReserveBlockModel> Blocks { get; set; }
        public E_OrderSource Source { get; set; }
   
    }
}
