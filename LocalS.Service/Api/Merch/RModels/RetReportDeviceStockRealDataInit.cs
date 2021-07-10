using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetReportDeviceStockRealDataInit
    {
        public RetReportDeviceStockRealDataInit()
        {
            this.OptionsByDevice = new List<OptionNode>();
        }

        public List<OptionNode> OptionsByDevice { get; set; }
    }
}
