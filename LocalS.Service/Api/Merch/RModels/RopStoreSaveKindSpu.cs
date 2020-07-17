using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopStoreSaveKindSpu
    {
        public string StoreId { get; set; }
        public string KindId { get; set; }
        public string ProductId { get; set; }
        public bool IsSynElseStore { get; set; }
    }
}
