using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos;

namespace LocalS.BLL
{
    public class ProductSkuModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }
        public List<ImgSet> DispalyImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string SpecDes { get; set; }
        public string BriefDes { get; set; }
    }
}
