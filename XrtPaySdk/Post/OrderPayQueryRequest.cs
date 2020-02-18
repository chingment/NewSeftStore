using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public class OrderPayQueryRequest : IApiPostRequest<OrderPayQueryRequestResult>
    {
        public Dictionary<string, string> PostData { get; set; }

        public OrderPayQueryRequest(Dictionary<string, string> postdata)
        {
            this.PostData = postdata;
        }
    }
}
