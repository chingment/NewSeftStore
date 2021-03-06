﻿using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class ProductSkuModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public string BriefDes { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DispalyImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string SpecDes { get; set; }
        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public bool IsOffSell { get; set; }
    }
}
