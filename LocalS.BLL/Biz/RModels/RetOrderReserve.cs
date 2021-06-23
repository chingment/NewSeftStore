using LocalS.Entity;
using Lumos;
using System.Collections.Generic;


namespace LocalS.BLL.Biz
{




    public class RetOrderReserve
    {
        public RetOrderReserve()
        {
            this.Orders = new List<Order>();
        }
        public List<Order> Orders { get; set; }
        public class Order
        {
            public string Id { get; set; }

            public string CumId { get; set; }
            public string ChargeAmount { get; set; }
        }
    }
}
