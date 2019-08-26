﻿using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPrdProductAdd
    {
        public string Name { get; set; }

        public string BarCode { get; set; }

        public string SimpleCode { get; set; }

        public List<ImgSet> DispalyImgUrls { get; set; }

        public decimal SalePrice { get; set; }

        public string DetailsDes { get; set; }

        public string SpecDes { get; set; }

        public string BriefDes { get; set; }

        public List<string> KindIds { get; set; }

        public List<string> SubjectIds
        {
            get; set;
        }

        public List<Spec> Specs { get; set; }

        public class Spec
        {
            public string Key { get; set; }

            public string Name { get; set; }

            public decimal SalePrice { get; set; }
        }
    }
}
