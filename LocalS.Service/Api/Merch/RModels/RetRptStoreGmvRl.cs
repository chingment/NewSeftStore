using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetRptStoreGmvRl
    {
        public RetRptStoreGmvRl()
        {
            this.Stores = new List<RptStoreGmvRlModel>();
        }

        public List<RptStoreGmvRlModel> Stores { get; set; }
    }
}
