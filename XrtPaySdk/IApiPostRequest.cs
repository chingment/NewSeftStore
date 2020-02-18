using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XrtPaySdk
{
    public interface IApiPostRequest<T>
    {
        Dictionary<string, string> PostData { get; set; }
    }
}
