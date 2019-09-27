
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetOrderPayStatusQuery
    {
        public string Id { get; set; }

        public string Sn { get; set; }

        public E_OrderStatus Status { get; set; }

        public RetOrderDetails Details { get; set; }

    }
}
