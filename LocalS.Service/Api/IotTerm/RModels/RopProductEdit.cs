﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.IotTerm
{
    public class RopProductEdit
    {
        public string spu_id { get; set; }
        public string name { get; set; }
        public string spu_code { get; set; }
        public List<string> spec_items { get; set; }
        public List<spec_sku> spec_skus { get; set; }
        public List<int> kind_ids { get; set; }
        public string brief_des { get; set; }
        public List<string> display_img_urls { get; set; }
        public List<string> details_des { get; set; }
    }
}
