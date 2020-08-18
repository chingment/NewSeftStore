using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class RefundRequest : IApiPostRequest<RefundResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public RefundRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
