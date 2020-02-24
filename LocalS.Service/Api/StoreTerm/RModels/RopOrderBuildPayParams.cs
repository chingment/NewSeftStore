using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopOrderBuildPayParams
    {
        public string OrderId { get; set; }
        public E_OrderPayCaller PayCaller { get; set; }
        public E_OrderPayPartner PayPartner { get; set; }
        public string CreateIp { get; set; }

    }
}
