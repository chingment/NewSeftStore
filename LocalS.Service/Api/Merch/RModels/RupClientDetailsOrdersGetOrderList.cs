using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupClientDetailsOrdersGetOrderList : RupBaseGetList
    {
        public string OrderSn { get; set; }

        public string ClientUserId{ get; set; }
    }
}
