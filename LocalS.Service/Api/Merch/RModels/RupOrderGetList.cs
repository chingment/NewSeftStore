using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupOrderGetList : RupBaseGetList
    {
        public string ClientUserName { get; set; }

        public string StoreId { get; set; }

        public string OrderSn { get; set; }

        public E_OrderStatus OrderStauts { get; set; }

        public string MachineId { get; set; }

        public string ClientUserId { get; set; }
    }
}
