using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public enum InvolveOp
    {
        Unknow = 0,
        GoodsHaveBeenDeliveredToCustomer = 1,
        GoodsNotHaveBeenDeliveredToCustomer = 2
    }
    public class RopOrderInvolvePickupFlow
    {
        public string UniqueId { get; set; }

        public InvolveOp InvolveOp { get; set; }

        public string Remark { get; set; }

    }
}
