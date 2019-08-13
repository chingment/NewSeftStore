using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeiXinSdk
{
    public interface IWxApiGetRequest<T>  where T : WxApiBaseResult
    {
        string ApiUrl { get; }

        IDictionary<string, string> GetUrlParameters();

    }
}
