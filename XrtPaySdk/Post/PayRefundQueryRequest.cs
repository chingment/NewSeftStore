using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class PayRefundQueryRequest : IApiPostRequest<PayRefundQueryRequestResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public PayRefundQueryRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
