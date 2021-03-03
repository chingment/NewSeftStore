using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class PdRentModel
    {
        public PdRentModel()
        {
            this.List = new List<SkuModel>();
        }

        public string Name { get; set; }
        public List<SkuModel> List { get; set; }
    }
}
