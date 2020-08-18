using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class PayRefundRequest : IApiPostRequest<PayRefundResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public PayRefundRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
