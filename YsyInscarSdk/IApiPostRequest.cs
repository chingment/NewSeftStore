using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsyInscarSdk
{
    public interface IApiPostRequest<T>
    {
        string ApiName { get; }                                                                                       

        IDictionary<string, string> GetUrlParameters();

        object PostData { get; set; }
    }
}
