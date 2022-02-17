using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class RopDeviceFill
    {
        public string DeviceId { get; set; }

        public Dictionary<string,object> Answers { get; set; }
    }
}
