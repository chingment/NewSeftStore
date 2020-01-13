using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetReportProductSkuDaySalesInit
    {
        public RetReportProductSkuDaySalesInit()
        {
            this.Machines = new List<MachineModel>();
            this.Stores = new List<StoreModel>();
        }

        public List<MachineModel> Machines { get; set; }

        public List<StoreModel> Stores { get; set; }
    }
}
