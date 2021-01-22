using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk
{
    public class WxApiGetWxACodeUnlimitResult : WxApiBaseResult
    {
        public string contentType { get; set; }

        public byte[] buffer { get; set; }
    }
}
