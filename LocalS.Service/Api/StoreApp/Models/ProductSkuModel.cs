using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ProductSkuModel
    {
        private bool _isOffSell = true;

        public string Id { get; set; }
        public string SpuId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string BriefDes { get; set; }
        public List<string> CharTags { get; set; }
        public List<SpecDes> SpecDes { get; set; }
        public string SpecIdx { get; set; }
        public List<SpecItem> SpecItems { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }
        public bool IsShowPrice { get; set; }
        public List<SpecIdxSku> SpecIdxSkus{ get; set; }
        public int CartQuantity { get; set; }
        public decimal RentMhPrice { get; set; }
        public decimal DepositPrice { get; set; }
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
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public E_SupReceiveMode SupReceiveMode { get; set; }
        public int SellQuantity { get; set; }
        public bool IsUseRent { get; set; }
        public decimal RentAmount { get; set; }
        public E_RentTermUnit RentTermUnit { get; set; }
        public string RentTermUnitText { get; set; }
        public decimal DepositAmount { get; set; }
        public bool IsMavkBuy { get; set; }
        public int KindId1 { get; set; }
        public int KindId2 { get; set; }
        public int KindId3 { get; set; }
    }
}
