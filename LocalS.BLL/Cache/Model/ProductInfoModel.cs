using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductInfoModel
    {
        public string Id { get; set; }
        public string Producer { get; set; }
        public string PinYinIndex { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public List<SpecItem> SpecItems { get; set; }
        public bool IsTrgVideoService { get; set; }
        public List<string> CharTags { get; set; }
        public List<ProductSkuStockModel> Stocks { get; set; }
    }
}
