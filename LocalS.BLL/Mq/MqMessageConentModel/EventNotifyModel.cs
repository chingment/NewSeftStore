using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class EventNotifyModel
    {
        public string AppId { get; set; }
        public string Operater { get; set; }
        public string TrgerId { get; set; }
        public string EventCode { get; set; }
        public string EventRemark { get; set; }
        public object EventContent { get; set; }

        public int EventMsgId { get; set; }

        public string EventMsgMode{ get; set; }
    }
}
