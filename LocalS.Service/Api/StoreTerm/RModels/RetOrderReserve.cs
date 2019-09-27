using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderReserve
    {
        public RetOrderReserve()
        {

        }

        public string OrderId { get; set; }
        public string OrderSn { get; set; }

        public string PayUrl { get; set; }
        public string ChargeAmount { get; set; }
    }

}
