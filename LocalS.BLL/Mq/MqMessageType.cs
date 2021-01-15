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
        PayTransResultNotify = 2,
        EventNotify = 3,
        PayRefundResultNotify = 4,
        OperateLog = 5
    }
}
