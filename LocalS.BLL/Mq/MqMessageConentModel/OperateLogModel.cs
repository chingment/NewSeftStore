using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class OperateLogModel
    {
        public string AppId { get; set; }
        public string Operater { get; set; }
        public string TrgerId { get; set; }
        public string EventCode { get; set; }
        public string EventRemark { get; set; }
    }
}
