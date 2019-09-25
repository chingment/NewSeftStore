using LocalS.Entity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class PrdProductModel2
    {
        public PrdProductModel2()
        {
            this.RefSku = new RefSkuModel();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public List<Lumos.ImgSet> DispalyImgUrls { get; set; }
        public string BriefDes { get; set; }
        public string DetailsDes { get; set; }
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
