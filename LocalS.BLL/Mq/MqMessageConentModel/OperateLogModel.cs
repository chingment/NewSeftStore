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
        public string AppId { get; set; }
        public Lumos.DbRelay.Enumeration.OperateType Type { get; set; }
        public string Remark { get; set; }
        public object Parms { get; set; }
    }
}
