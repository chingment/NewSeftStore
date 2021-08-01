using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopReplenishGetPlans
    {
        public string DeviceId { get; set; }
        public int Page { get; set; }
        public int Limit { get; set; }
    }
}
