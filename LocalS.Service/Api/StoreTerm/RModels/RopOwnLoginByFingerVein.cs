using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopOwnLoginByFingerVein
    {
        public string VeinData { get; set; }
        public string AppId { get; set; }
        public Enumeration.LoginWay LoginWay { get; set; }
        public string DeviceId { get; set; }
    }
}
