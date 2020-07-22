using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPrdProductEdit
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string BarCode { get; set; }

        public string SimpleCode { get; set; }

        public List<ImgSet> DisplayImgUrls { get; set; }

        public decimal ShowPrice { get; set; }

        public decimal SalePrice { get; set; }

        public bool IsUnifyUpdateSalePrice { get; set; }
        public List<ImgSet> DetailsDes { get; set; }

        public string BriefDes { get; set; }

        public bool IsTrgVideoService { get; set; }

        public List<string> CharTags { get; set; }

        public List<int> KindIds { get; set; }

        public List<string> SubjectIds
        {
            get; set;
        }

        public List<Sku> Skus { get; set; }
        public class Sku
        {
            public string Id { get; set; }
            public List<SpecDes> SpecDes { get; set; }

            public decimal SalePrice { get; set; }

            public string BarCode { get; set; }

            public string CumCode { get; set; }

            public bool IsOffSell { get; set; }
        }

        public class SpecDes
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
