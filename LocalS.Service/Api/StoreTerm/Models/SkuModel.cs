using LocalS.BLL;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class SkuModel
    {
        private bool _isOffSell = true;


        public string SkuId { get; set; }
        public string SpuId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }
        public bool IsShowPrice { get; set; }
        public string SpecDes { get; set; }
        public bool IsOffSell
        {
            get
            {
                return _isOffSell;
            }
            set
            {
                _isOffSell = value;
            }
        }
        public int SellQuantity { get; set; }
        public bool IsTrgVideoService { get; set; }
        public List<string> CharTags { get; set; }
    }
}
