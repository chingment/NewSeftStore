using LocalS.BLL.Biz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetStockSettingGetSlots
    {
        public RetStockSettingGetSlots()
        {
            this.Slots = new Dictionary<string, SlotModel>();
        }

        public string RowColLayout { get; set; }

        public int SlotMaxQuantity { get; set; }

        public Dictionary<string, SlotModel> Slots { get; set; }
    }
}
