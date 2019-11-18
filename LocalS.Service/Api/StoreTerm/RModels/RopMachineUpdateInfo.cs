using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopMachineUpdateInfo
    {
        public string MachineId { get; set; }
        public int DataType { get; set; }
        public object DataContent { get; set; }


        public class DataContentByLatLng
        {
            public float Lng { get; set; }

            public float Lat { get; set; }
        }
    }
}
