using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupLogGetListByRelStock : RupBaseGetList
    {
        public string SkuId { get; set; }
        public string StoreId { get; set; }
        public string ShopId { get; set; }
        public string DeviceId { get; set; }
        public string CabinetId { get; set; }
        public string SlotId { get; set; }

    }
}
