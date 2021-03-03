using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class CartDataModel
    {
        public CartDataModel()
        {
            this.Blocks = new List<BlockModel>();
        }

        public List<BlockModel> Blocks { get; set; }

        public int Count { get; set; }

        public decimal SumPrice { get; set; }

        public int CountBySelected { get; set; }

        public decimal SumPriceBySelected { get; set; }

        public class BlockModel
        {

            public string TagName { get; set; }

            public List<SkuModel> Skus { get; set; }

            public E_ShopMode ShopMode { get; set; }

        }

        public class SkuModel
        {
            public string Id { get; set; }
            public string SpuId { get; set; }
            public string Name { get; set; }
            public string MainImgUrl { get; set; }
            public bool IsOffSell { get; set; }
            public decimal SalePrice { get; set; }
            public decimal ShowPrice { get; set; }
            public string BriefInfo { get; set; }
            public List<Lumos.ImgSet> DisplayImgUrls { get; set; }
            public string DetailsDes { get; set; }
            public string SpecDes { get; set; }
            public string CartId { get; set; }
            public int Quantity { get; set; }
            public bool Selected { get; set; }
            public decimal SumPrice { get; set; }
            public E_ShopMode ShopMode { get; set; }
            public string ShopId { get; set; }

        }
    }
}
