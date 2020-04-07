using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopMachineHandleRunExItems
    {

        public RopMachineHandleRunExItems()
        {
            this.ExOrders = new List<Order>();
        }

        public string MachineId { get; set; }

        public List<Order> ExOrders { get; set; }

        public class Order
        {
            public Order()
            {
                this.DetailItems = new List<OrderDetailItem>();
            }

            public string Id { get; set; }

            public List<OrderDetailItem> DetailItems { get; set; }

        }

        public class OrderDetailItem
        {
            public string UniqueId { get; set; }
            public int PickupStatus { get; set; }
        }
    }
}
