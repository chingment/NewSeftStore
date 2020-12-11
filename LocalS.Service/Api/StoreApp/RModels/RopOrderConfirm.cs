﻿using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RopOrderConfirm
    {
        public List<string> OrderIds { get; set; }
        public string StoreId { get; set; }
        public List<OrderConfirmProductSkuModel> ProductSkus { get; set; }
        public E_AppCaller Caller { get; set; }
        public List<string> CouponIds { get; set; }
        public E_ShopMethod ShopMethod { get; set; }
    }
}
