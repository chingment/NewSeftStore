using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class DeviceEventByDeviceStatusModel : DeviceEventBaseModel
    {
        public string Activity { get; set; }
        public long UpKb { get; set; }
        public long DownKb { get; set; }
        public string Status { get; set; }
    }
}
