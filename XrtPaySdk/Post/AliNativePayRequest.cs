using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class AliNativePayRequest : IApiPostRequest<AliNativePayRequestResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public AliNativePayRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
