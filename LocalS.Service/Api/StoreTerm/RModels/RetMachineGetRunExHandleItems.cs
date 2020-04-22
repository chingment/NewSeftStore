using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetMachineGetRunExHandleItems
    {
        public RetMachineGetRunExHandleItems()
        {
            this.ExReasons = new List<ExReason>();
            this.ExOrders = new List<Order>();
        }

        public List<ExReason> ExReasons { get; set; }
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
            public string ProductId { get; set; }
            public string MainImgUrl { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string UniqueId { get; set; }
            public string SlotId { get; set; }
            public bool CanHandle { get; set; }
            public int SignStatus { get; set; }
        }

        public class ExReason
        {

            public string Id { get; set; }
            public string Title { get; set; }
            public bool IsChecked { get; set; }
        }
    }
}
