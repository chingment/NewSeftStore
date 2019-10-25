using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetRptProductSkuSaleRl
    {
        public RetRptProductSkuSaleRl()
        {
            this.ProductSkus = new List<RptProductSkuSaleRlModel>();
        }

        public List<RptProductSkuSaleRlModel> ProductSkus { get; set; }
    }
}
