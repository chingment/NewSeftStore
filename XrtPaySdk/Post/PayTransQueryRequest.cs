using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class PayTransQueryRequest : IApiPostRequest<PayTransQueryRequestResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public PayTransQueryRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
