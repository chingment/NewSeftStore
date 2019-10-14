using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductInfoModel2
    {
        public ProductInfoModel2()
        {

        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string BriefDes { get; set; }

        public string RefSkuId { get; set; }
    }
}
