﻿using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderSearchByPickupCode
    {
        public string Id { get; set; }
        public E_OrderStatus Status { get; set; }
        public List<OrderProductSkuByPickupModel> ProductSkus { get; set; }

    }
}
