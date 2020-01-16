using LocalS.Service.UI;
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
            this.OptionsSellChannels = new List<OptionNode>();
        }

        public List<OptionNode> OptionsSellChannels { get; set; }
    }
}
