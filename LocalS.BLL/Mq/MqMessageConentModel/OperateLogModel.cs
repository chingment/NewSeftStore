using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class OperateLogModel
    {
        public string Operater { get; set; }
        public string MachineId { get; set; }
        public string AppId { get; set; }
        public string EventCode { get; set; }
        public string Remark { get; set; }
        public object Parms { get; set; }
    }
}
