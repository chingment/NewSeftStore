using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public enum MqMessageType
    {
        Unknow = 0,
        OrderReserve = 1,
        PayResultNotify = 2
    }
}
