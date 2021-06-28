using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class RopProductAdd
    {
        public string name { get; set; }
        public string spu_code { get; set; }
        public List<string> spec_items { get; set; }
        public List<spec_sku> spec_skus { get; set; }
        public List<int> kind_ids { get; set; }
        public string brief_des { get; set; }
        public List<string> display_img_urls { get; set; }
        public List<string> details_des { get; set; }
        public class spec_sku
        {
            public string cum_code { get; set; }
            public string bar_code { get; set; }
            public decimal sale_price { get; set; }
            public List<string> spec_val { get; set; }
        }
    }
}
