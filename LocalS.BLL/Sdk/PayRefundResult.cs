using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class PayRefundResult
    {
        public string Status { get; set; }
        public string PayTransId { get; set; }
        public string PayRefundId { get; set; }
        public string PayPartnerPayRefundId { get; set; }
        public string RefundChannel { get; set; }
        public int RefundFee{ get; set; }
        public int CouponRefundFee { get; set; }
    }
}
