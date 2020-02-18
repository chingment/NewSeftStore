using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XrtPaySdk
{
    public class WxNativePayRequest : IApiPostRequest<WxNativePayRequestResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public WxNativePayRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
