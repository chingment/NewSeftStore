using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopCouponSend
    {
        public string[] CouponIds { get; set; }

        public int Quantity { get; set; }

        public string[] ClientUserIds { get; set; }
    }
}
