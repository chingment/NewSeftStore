using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetCouponMy
    {
        public RetCouponMy()
        {
            this.Coupons = new List<UserCouponModel>();
        }

        public List<UserCouponModel> Coupons { get; set; }
    }
}
