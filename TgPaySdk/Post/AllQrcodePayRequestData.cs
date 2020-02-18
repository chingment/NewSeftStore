using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgPaySdk
{
    public class AllQrcodePayRequestData : BaseRequestData
    {
        public string payMoney { get; set; }
        public string lowOrderId { get; set; }
        public string body { get; set; }
        public string attach { get; set; }
        public string lowCashier { get; set; }
        public string notifyUrl { get; set; }
        public string returnUrl { get; set; }
    }
}
