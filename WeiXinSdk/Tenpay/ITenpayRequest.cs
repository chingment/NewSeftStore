using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinSdk.Tenpay
{
    public interface ITenpayRequest
    {
        SortedDictionary<string, string> DoPost(WxAppInfoConfig config, ITenpayPostApi obj, bool isUserCert = false);

        string ReturnContent { get; }

    }
}
