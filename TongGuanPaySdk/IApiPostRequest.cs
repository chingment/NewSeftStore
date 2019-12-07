using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TongGuanPaySdk
{
    public interface IApiPostRequest<T>
    {
        string ApiUrl { get; }

        Object PostData { get; set; }
    }
}
