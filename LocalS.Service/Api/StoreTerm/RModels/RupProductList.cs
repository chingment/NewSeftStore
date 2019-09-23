using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RupProductList : RupBaseGetList
    {
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string MachineId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string KindId { get; set; }
    }
}
