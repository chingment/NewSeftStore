using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlipaySdk
{
    public class UnifiedOrder
    {
        public string store_id { get; set; }
        public string subject { get; set; }
        public string out_trade_no { get; set; }
        public string total_amount { get; set; }
        public string timeout_express { get; set; }
        //public string passback_params { get; set; }
    }
}
