﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class RetPayResultQuery
    {
        public string OrderId { get; set; }
        public string OrderSn { get; set; }
        public E_OrderStatus Status { get; set; }

    }
}
