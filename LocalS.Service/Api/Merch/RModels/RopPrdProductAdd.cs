using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPrdProductAdd
    {
        public RopPrdProductAdd()
        {
            this.Skus = new List<Sku>();
            this.SpecItems = new List<SpecItem>();
        }

        public string Name { get; set; }
        public string SimpleCode { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public List<string> KindIds { get; set; }
        public List<string> SubjectIds
        {
            get; set;
        }
        public List<Sku> Skus { get; set; }

        public List<SpecItem> SpecItems { get; set; }

        public class Sku
        {
            public string Id { get; set; }
            public string CumCode { get; set; }
            public string BarCode { get; set; }
            public string SpecDes { get; set; }

            public decimal SalePrice { get; set; }
        }
        public class SpecItem
        {
            public SpecItem()
            {
                this.Values = new List<string>();
            }

            public string Name { get; set; }
            public List<string> Values { get; set; }
        }
    }
}
