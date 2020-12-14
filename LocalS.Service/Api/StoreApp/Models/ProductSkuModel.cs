using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ProductSkuModel
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string BriefDes { get; set; }
        public List<string> CharTags { get; set; }
        public List<SpecDes> SpecDes { get; set; }
        public string SpecIdx { get; set; }
        public List<SpecItem> SpecItems { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }
        public bool IsShowPrice { get; set; }
        public bool IsOffSell { get; set; }
        public List<SpecIdxSku> SpecIdxSkus{ get; set; }

        public int CartQuantity { get; set; }
}
}
