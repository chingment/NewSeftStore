using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductSpuInfoBySearchModel
    {
        public string SpuId { get; set; }
        public string SpuCode { get; set; }
        public string BarCode { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public bool IsSupRentService { get; set; }
    }
}
