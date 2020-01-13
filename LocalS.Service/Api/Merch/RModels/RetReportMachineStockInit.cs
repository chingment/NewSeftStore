using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetReportMachineStockInit
    {
        public RetReportMachineStockInit()
        {
            this.Machines = new List<MachineModel>();
        }

        public List<MachineModel> Machines { get; set; }
    }
}
