﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetCouponRevCenterSt
    {
        public RetCouponRevCenterSt()
        {
            this.Coupons = new List<CouponModel>();
        }

        public string TopImgUrl { get; set; }

        public List<CouponModel> Coupons { get; set; }
    }
}