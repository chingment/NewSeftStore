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
        CustomJsonResult Send(string registrationid, string cmd, object content);

        CustomJsonResult QueryStatus(string registrationid, string msgId);
    }
}
