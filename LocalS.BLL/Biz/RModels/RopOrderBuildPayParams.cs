using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class RopOrderBuildPayParams
    {
        public RopOrderBuildPayParams()
        {
            this.Orders = new List<Order>();
            this.Blocks = new List<OrderReserveBlockModel>();
        }
        public string AppId { get; set; }
        public List<Order> Orders { get; set; }
        public E_PayCaller PayCaller { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string CreateIp { get; set; }

        public List<OrderReserveBlockModel> Blocks { get; set; }

        public class Order
        {
            public string Id { get; set; }
        }

    }
}
