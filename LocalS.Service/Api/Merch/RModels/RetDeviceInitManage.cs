using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetDeviceInitManage
    {
        public RetDeviceInitManage()
        {
            this.Devices = new List<DeviceModel>();
        }

        public DeviceModel CurDevice { get; set; }

        public List<DeviceModel> Devices { get; set; }
    }
}
