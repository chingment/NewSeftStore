using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopReplenishConfirmReplenish
    {

        public string PlanId { get; set; }
        public string PlanDeviceId { get; set; }
        public string DeviceId { get; set; }
        public List<SlotModel> Slots { get; set; }

        public class SlotModel
        {
            public string SlotId { get; set; }
            public string CabinetId { get; set; }
            public int RealRshQuantity { get; set; }
        }
    }
}
