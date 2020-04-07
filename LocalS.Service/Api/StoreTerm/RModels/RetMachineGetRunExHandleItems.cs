﻿using System;
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
            this.ExOrders = new List<Order>();
        }

        public List<Order> ExOrders { get; set; }

        public class Order
        {
            public Order()
            {
                this.DetailItems = new List<OrderDetailItem>();
            }

            public string Id { get; set; }
            public string Sn { get; set; }
            public string SubId { get; set; }
            public string SubSn { get; set; }
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
        }
    }
}