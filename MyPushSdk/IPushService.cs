using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyPushSdk
{

    public interface IPushService
    {
        CustomJsonResult Send(string deviceId, string msgId, string method, object pms);

        CustomJsonResult QueryStatus(string deviceId, string msgId);
    }
}
