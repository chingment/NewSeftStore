using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class WxJsPayRequest : IApiPostRequest<WxJsPayRequestResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public WxJsPayRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
