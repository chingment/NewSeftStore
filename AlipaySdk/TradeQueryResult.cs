using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlipaySdk
{
    public class TradeQueryResult
    {
        public Response alipay_trade_query_response { get; set; }

        public string sign { get; set; }

        public class Response
        {
            public string code { get; set; }
            public string msg { get; set; }
            public string trade_no { get; set; }
            public string out_trade_no { get; set; }
            public string buyer_logon_id { get; set; }
            public string trade_status { get; set; }
            public string buyer_user_name { get; set; }
            public string subject { get; set; }
            public string body { get; set; }
        }
    }
}
