using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class RopEgyContactSave
    {
        public string  Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsEnable { get; set; }
    }
}
