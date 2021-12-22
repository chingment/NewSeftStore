using LocalS.BLL.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetReportSkuSalesDateHisInit
    {
        public RetReportSkuSalesDateHisInit()
        {
            this.OptionsStores = new List<OptionNode>();
        }

        public List<OptionNode> OptionsStores { get; set; }
    }
}
