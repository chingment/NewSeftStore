using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetMachineGetSlots
    {
        public RetMachineGetSlots()
        {
            this.Slots = new Dictionary<string, SlotModel>();
        }

        public CabineRowColLayoutModel RowColLayout { get; set; }

        public Dictionary<string, SlotModel> Slots { get; set; }
    }
}
