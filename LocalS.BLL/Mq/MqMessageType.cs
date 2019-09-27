using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Mq
{
    public enum MqMessageType
    {
        Unknow = 0,
        OrderReserve = 1,
        OrderCancle = 2,
        OrderPayCompleted = 3
    }
}
