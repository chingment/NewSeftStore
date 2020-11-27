using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupCouponGetList : RupBaseGetList
    {
        public string Name { get; set; }

        public E_Coupon_Category Category { get; set; }
    }
}
