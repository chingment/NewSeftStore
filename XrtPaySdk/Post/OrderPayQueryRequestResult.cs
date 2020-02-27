using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    [XmlRoot]
    public class OrderPayQueryRequestResult : BaseRequestResult
    {
        public string trade_state { get; set; }
        public string trade_type { get; set; }
        public string openid { get; set; }
        public string is_subscribe { get; set; }
        public string transaction_id { get; set; }
        public string out_trade_no { get; set; }
        public string total_fee { get; set; }
        public string coupon_fee { get; set; }
        public string fee_type { get; set; }
        public string attach { get; set; }
        public string bank_type { get; set; }
        public string bank_billno { get; set; }
        public string time_end { get; set; }
    }
}
