using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetRptSkuSaleRl
    {
        public RetRptSkuSaleRl()
        {
            this.Skus = new List<object>();
        }

        public List<object> Skus { get; set; }
    }
}
