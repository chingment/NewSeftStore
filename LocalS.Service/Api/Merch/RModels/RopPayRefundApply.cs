using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPayRefundApply
    {
        public string OrderId { get; set; }
        public E_PayRefundMethod Method { get; set; }
        public string Remark { get; set; }
        public decimal Amount { get; set; }
    }
}
