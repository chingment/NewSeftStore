using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopStoreRemoveKind
    {
        public string StoreId { get; set; }
        public string Id { get; set; }
        public bool IsSynElseStore { get; set; }
    }
}
