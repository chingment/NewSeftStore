﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupOrderList
    {
        public string StoreId { get; set; }
        public int PageIndex { get; set; }

        public E_OrderStatus Status { get; set; }

        public E_AppCaller Caller { get; set; }
    }
}
