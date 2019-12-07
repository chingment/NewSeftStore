using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TongGuanPaySdk
{
    public class AllQrcodePayAsynNotifyResult
    {
        public string lowOrderId { get; set; }
        public string upOrderId { get; set; }
        public string account { get; set; }
        public string merchantId { get; set; }
        public string payMoney { get; set; }
        public string channelID { get; set; }
        public string state { get; set; }
        public string orderDesc { get; set; }
        public string payTime { get; set; }
        public string openid { get; set; }
        public string attach { get; set; }
        public string sign { get; set; }
    }
}
