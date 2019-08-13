using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsyInscarSdk
{
    public interface IApiGetRequest<T>
    {
        string UserCode { get; set; }
        string Key { get; set; }

        string ApiName { get; }

        IDictionary<string, string> GetUrlParameters();
    }
}
