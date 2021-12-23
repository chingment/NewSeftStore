using LocalS.BLL;
using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RetDeviceInitData
    {
        public RetDeviceInitData()
        {
            this.Device = new DeviceModel();
        }
        public DeviceModel Device { get; set; }
        public object CustomData { get; set; }
    }
}
