﻿using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupCouponAdd
    {
        public string Name { get; set; }
        public E_Coupon_Category Category { get; set; }
        public E_Coupon_ShopMode ShopMode { get; set; }
        public int IssueQuantity { get; set; }
        public E_Coupon_FaceType FaceType { get; set; }
        public decimal FaceValue { get; set; }
        public int PerLimitNum { get; set; }
        public decimal AtLeastAmount { get; set; }
        public string[] ValidDate { get; set; }
        public E_Coupon_UseAreaType UseAreaType { get; set; }
        public E_Coupon_UseMode UseMode { get; set; }
        public string UseAreaValue { get; set; }
        public string Description { get; set; }
        public bool IsSuperposition { get; set; }
    }
}
