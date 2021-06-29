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
        public string DeviceId { get; set; }
        public string OrderId { get; set; }
        public E_PayCaller PayCaller { get; set; }
        public E_PayPartner PayPartner { get; set; }
        public string CreateIp { get; set; }

    }
}
