using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetMachineGetSlotStock
    {
        public RetMachineGetSlotStock()
        {
            this.SlotProductSkus = new List<SlotProductSkuModel>();
        }

        public List<SlotProductSkuModel> SlotProductSkus { get; set; }
    }
}
