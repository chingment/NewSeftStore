using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class StoreModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public double Distance { get; set; }
        public string DistanceMsg { get; set; }
    }
}
