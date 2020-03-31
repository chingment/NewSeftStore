using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Mq
{
    public class MachineEventNotifyModel
    {
        public string AppId { get; set; }
        public string MachineId { get; set; }
        public string EventCode { get; set; }
        public double Lng { get; set; }
        public double Lat { get; set; }
        public string Operater { get; set; }
        public object Content { get; set; }
    }
}
