﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class PayResult
    {
        public bool IsPaySuccess { get; set; }
        public string OrderSn { get; set; }
        public string PayPartnerOrderSn { get; set; }
        public E_OrderPayWay OrderPayWay { get; set; }
        public string ClientUserName { get; set; }
    }
}