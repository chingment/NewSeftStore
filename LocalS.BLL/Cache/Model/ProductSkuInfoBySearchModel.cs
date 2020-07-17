using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductSkuInfoBySearchModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string CumCode { get; set; }
        public string BarCode { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }

        public string SpecDes { get; set; }
    }
}
