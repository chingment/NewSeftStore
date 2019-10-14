using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class StoreModel
    {
        public string Id { get; set; }

        public string MerchId { get; set; }

        public string[] MachineIds { get; set; }
    }
}
