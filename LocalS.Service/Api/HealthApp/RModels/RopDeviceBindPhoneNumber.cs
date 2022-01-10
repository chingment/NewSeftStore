using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.HealthApp
{
    public class RopDeviceBindPhoneNumber
    {
        public string DeviceId { get; set; }

        public string PhoneNumber { get; set; }

        public string ValidCode { get; set; }
    }
}
