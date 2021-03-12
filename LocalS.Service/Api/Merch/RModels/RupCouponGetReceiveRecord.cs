using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupCouponGetReceiveRecord:RupBaseGetList
    {
        public string CouponId { get; set; }

        public string NickName { get; set; }
    }
}
