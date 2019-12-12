using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopStockSettingSaveCabinetRowColLayout
    {
        public string MachineId { get; set; }

        public int CabinetId { get; set; }
        public int[] CabinetRowColLayout { get; set; }


    }
}
