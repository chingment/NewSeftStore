﻿using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductSkuInfoAndStockModel
    {
        public ProductSkuInfoAndStockModel()
        {
            this.Stocks = new List<ProductSkuStockModel>();
        }

        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public decimal SalePrice { get; set; }
        public decimal SalePriceByVip { get; set; }
        public bool IsShowPrice { get; set; }
        public string SpecDes { get; set; }
        public bool IsOffSell { get; set; }
        public List<ProductSkuStockModel> Stocks { get; set; }
    }
}
