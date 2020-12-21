using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetIndexSugProducts
    {
        public RetIndexSugProducts()
        {
            this.PdRent = new PdRentModel();
            this.PdArea = new PdAreaModel();
        }
        public PdRentModel PdRent { get; set; }
        public PdAreaModel PdArea { get; set; }
    }
}
