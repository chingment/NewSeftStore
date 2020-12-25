using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{

    public class SpecIdxSku
    {
        public string SkuId { get; set; }

        public string SpecIdx { get; set; }
    }

    public class ProductSpuInfoModel
    {
        public ProductSpuInfoModel()
        {
            this.DisplayImgUrls = new List<ImgSet>();
            this.DetailsDes = new List<ImgSet>();
            this.SpecItems = new List<SpecItem>();
            this.SpecIdxSkus = new List<SpecIdxSku>();
            this.CharTags = new List<string>();
        }
        public string Id { get; set; }
        public string Producer { get; set; }
        public string PinYinIndex { get; set; }
        public string Name { get; set; }
        public string SpuCode { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public List<SpecItem> SpecItems { get; set; }
        public bool IsTrgVideoService { get; set; }
        public bool IsRevService { get; set; }
        public List<string> CharTags { get; set; }
        public List<SpecIdxSku> SpecIdxSkus { get; set; }

        public int KindId1 { get; set; }
        public int KindId2 { get; set; }
        public int KindId3 { get; set; }
    }
}
