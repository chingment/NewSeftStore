using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupPrdProductGetList : RupBaseGetList
    {
        public string Name { get; set; }

        public string SkuCode { get; set; }

        public string BarCode { get; set; }
    }
}
