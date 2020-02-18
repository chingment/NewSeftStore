using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgPaySdk
{
    public class AllQrcodePayRequest:IApiPostRequest<AllQrcodePayRequestResult>
    {
        public AllQrcodePayRequest(Object postdata)
        {
            this.PostData = postdata;
        }

        public Object PostData { get; set; }

        public string ApiUrl
        {
            get
            {
                return "/tgPosp/services/payApi/allQrcodePay";
            }
        }
    }
}
