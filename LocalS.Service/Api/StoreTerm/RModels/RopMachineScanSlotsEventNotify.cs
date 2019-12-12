using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopMachineScanSlotsEventNotify
    {
        public string MachineId { get; set; }

        public int Status { get; set; }

        public string Remark { get; set; }
    }
}
