using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetMachineInitManageStock
    {
        public RetMachineInitManageStock()
        {
            this.OptionsCabinets = new List<OptionNode>();
        }

        public List<OptionNode> OptionsCabinets { get; set; }
    }
}
