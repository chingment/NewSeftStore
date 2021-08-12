using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopDeviceSetSysParams
    {
        public string Id { get; set; }

        public List<CbLightModel> CbLight { get; set; }

        public class CbLightModel
        {
            public string Start { get; set; }
            public string End { get; set; }
            public int Value { get; set; }
        }
    }
}
