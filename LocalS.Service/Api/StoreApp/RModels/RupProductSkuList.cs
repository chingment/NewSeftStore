﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupProductSkuList
    {
        public string StoreId { get; set; }
        public int PageIndex { get; set; }
        public string KindId { get; set; }

        public string SubjectId { get; set; }

        public string Name { get; set; }
    }
}
