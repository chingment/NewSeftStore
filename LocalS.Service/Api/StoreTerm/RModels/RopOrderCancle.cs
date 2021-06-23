using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopOrderCancle
    {

        public string MachineId { get; set; }
        public string OrderId { get; set; }

        public string Reason { get; set; }
         
        public E_OrderCancleType Type { get; set; }
    }
}
