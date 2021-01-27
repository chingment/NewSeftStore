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
            this.Stores = new List<object>();
        }

        public List<object> Stores { get; set; }
    }
}
