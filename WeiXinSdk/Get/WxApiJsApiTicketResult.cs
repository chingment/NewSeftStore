using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk
{
    public class WxApiJsApiTicketResult : WxApiBaseResult
    {
        public string ticket { get; set; }

        public int expires_in { get; set; }
    }
}
