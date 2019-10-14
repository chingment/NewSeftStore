using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetMachineGetSlotStocks
    {
        public RetMachineGetSlotStocks()
        {
            this.SlotStocks = new Dictionary<string, SlotStockModel>();
        }

        public Dictionary<string, SlotStockModel> SlotStocks { get; set; }
    }
}
