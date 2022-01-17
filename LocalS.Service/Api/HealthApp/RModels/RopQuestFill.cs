using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class RopQuestFill
    {
        public string DeviceId { get; set; }

        public Dictionary<string,string> Answers { get; set; }
    }
}
