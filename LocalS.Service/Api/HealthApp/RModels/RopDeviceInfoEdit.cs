using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class RopDeviceInfoEdit
    {
        public string SvUserId { get; set; }
        public Dictionary<string, object> Answers { get; set; }
    }
}
