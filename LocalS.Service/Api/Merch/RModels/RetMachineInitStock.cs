using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetMachineInitStock
    {
        public RetMachineInitStock()
        {
            this.CurMachine = new MachineModel();
            this.Machines = new List<MachineModel>();
        }

        public MachineModel CurMachine { get; set; }

        public List<MachineModel> Machines { get; set; }
    }
}
