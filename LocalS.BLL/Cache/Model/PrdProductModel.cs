using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos;

namespace LocalS.BLL
{
    public class PrdProductModel
    {
        public PrdProductModel()
        {
            this.AllSkus = new List<Sku>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DispalyImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public Sku RefSku { get; set; }
        public List<Sku> AllSkus { get; set; }
        public class Sku
        {
            public string Id { get; set; }

            public decimal SalePrice { get; set; }

            public decimal ShowPrice { get; set; }

            public bool IsShowPrice { get; set; }

            public string SpecDes { get; set; }
        }
    }
}
