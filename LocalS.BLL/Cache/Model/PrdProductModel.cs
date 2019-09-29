using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lumos;
using LocalS.Entity;

namespace LocalS.BLL
{
    public class PrdProductModel
    {
        public PrdProductModel()
        {

        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public string DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public RefSkuModel RefSku { get; set; }
        public class RefSkuModel
        {
            public string Id { get; set; }
            public E_ReceptionMode ReceptionMode { get; set; }
            public int SumQuantity { get; set; }
            public int LockQuantity { get; set; }
            public int SellQuantity { get; set; }
            public bool IsOffSell { get; set; }
            public decimal SalePrice { get; set; }
            public decimal SalePriceByVip { get; set; }
            public decimal ShowPrice { get; set; }
            public string SpecDes { get; set; }
            public bool IsShowPrice { get; set; }
        }
    }
}
