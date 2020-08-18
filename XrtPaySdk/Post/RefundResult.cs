using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    [XmlRoot("xml")]
    public class RefundResult : BaseRequestResult
    {
        public string transaction_id { get; set; }
        public string out_trade_no { get; set; }
        public string out_refund_no { get; set; }
        public string refund_id { get; set; }
        public string refund_channel { get; set; }
        public int refund_fee { get; set; }
        public int coupon_refund_fee { get; set; }
    }
}
