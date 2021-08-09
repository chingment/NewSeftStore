using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Push
{
    public class MqttConnectConfig
    {
        public string Type { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ClientId { get; set; }
        public string ProductKey { get; set; }
        public string DeviceName { get; set; }
        public string DeviceSecret { get; set; }


    }
}
