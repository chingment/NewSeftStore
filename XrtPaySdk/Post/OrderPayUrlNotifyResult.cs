using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    [XmlRoot("xml")]
    public class OrderPayUrlNotifyResult : BaseRequestResult
    {
        public string openid { get; set; }
        public string trade_type { get; set; }
        public int is_subscribe { get; set; }
        public int pay_result { get; set; }
        public string transaction_id { get; set; }
        public string out_transaction_id { get; set; }

        public int sub_is_subscribe { get; set; }

        public string sub_appid { get; set; }

        public string sub_openid { get; set; }
        public string out_trade_no { get; set; }
        public int total_fee { get; set; }
        public int coupon_fee { get; set; }
        public string fee_type { get; set; }
        public string attach { get; set; }
        public string bank_type { get; set; }
        public string bank_billno { get; set; }
        public string time_end { get; set; }
    }
}
