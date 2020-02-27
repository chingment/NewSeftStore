using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopMachineEventNotify
    {
        public string AppId { get; set; }
        public string DeviceId { get; set; }
        public string MachineId { get; set; }
        public object Content { get; set; }
        public E_MachineEventType Type { get; set; }
    }
}
