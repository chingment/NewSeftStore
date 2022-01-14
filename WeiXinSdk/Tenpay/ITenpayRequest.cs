using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWeiXinSdk.Tenpay
{
    public interface ITenpayRequest
    {
        SortedDictionary<string, string> DoPost(WxAppConfig config, ITenpayPostApi obj, bool isUserCert = false);

        string ReturnContent { get; }

    }
}
