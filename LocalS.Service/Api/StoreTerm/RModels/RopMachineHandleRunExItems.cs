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
            this.Orders = new List<Order>();
        }

        public string MachineId { get; set; }
        public string Rermark { get; set; }
        public List<Order> Orders { get; set; }
        public class Order
        {
            public Order()
            {
                this.UniqueItems = new List<OrderUniqueItem>();
            }

            public string Id { get; set; }

            public List<OrderUniqueItem> UniqueItems { get; set; }
        }


        public class OrderUniqueItem
        {
            public string UniqueId { get; set; }
            public int SignStatus { get; set; }
        }
    }
}
