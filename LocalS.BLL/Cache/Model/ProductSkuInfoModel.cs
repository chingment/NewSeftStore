using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class ProductSkuInfoModel
    {
        public string Id { get; set; }
        public string BarCode { get; set; }
        public string SpuCode { get; set; }
        public string CumCode { get; set; }
        public string Producer { get; set; }
        public string PinYinIndex { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public List<SpecDes> SpecDes { get; set; }
        public string BriefDes { get; set; }
        public List<SpecItem> SpecItems { get; set; }
        public string SpecIdx { get; set; }
        public bool IsTrgVideoService { get; set; }
        public List<string> CharTags { get; set; }
        public List<SpecIdxSku> SpecIdxSkus { get; set; }
        public List<ProductSkuStockModel> Stocks { get; set; }

        public int KindId1 { get; set; }
        public int KindId2 { get; set; }
        public int KindId3 { get; set; }

        public int SumQuantity { get; set; }
        public int LockQuantity { get; set; }
        public int SellQuantity { get; set; }
        public bool IsOffSell { get; set; }
        public decimal SalePrice { get; set; }
        public bool IsUseRent { get; set; }
        public decimal RentMhPrice { get; set; }
        public decimal DepositPrice { get; set; }
    }
}
