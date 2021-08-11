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
        public decimal RefundFee { get; set; }
        public decimal CouponRefundFee { get; set; }
        public DateTime? RefundTime { get; set; }
        public string Message { get; set; }
    }
}
