using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopStockSettingConfirmRshPlanQuantity
    {
        public string PlanCumCode { get; set; }
        public string DeviceId { get; set; }
        public string CabinetId { get; set; }
        public Dictionary<string, SlotModel> Slots { get; set; }

        public class SlotModel
        {
            public string SlotId { get; set; }
            public string StockId { get; set; }
            public string RshQuantity { get; set; }
            public int Version { get; set; }
        }
    }
}
