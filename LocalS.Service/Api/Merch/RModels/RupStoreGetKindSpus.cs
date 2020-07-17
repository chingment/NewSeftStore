using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupStoreGetKindSpus:RupBaseGetList
    {
        public string StoreId { get; set; }
        public string KindId { get; set; }
    }
}
