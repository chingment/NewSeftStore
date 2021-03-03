using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{


    public class RopProductAdd
    {
        public RopProductAdd()
        {
            this.Skus = new List<Sku>();
            this.SpecItems = new List<SpecItem>();
        }

        public string Name { get; set; }
        public string SpuCode { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public string BriefDes { get; set; }
        public List<int> KindIds { get; set; }
        public bool IsTrgVideoService { get; set; }
        public bool IsRevService { get; set; }
        public bool IsSupRentService { get; set; }
        public bool IsMavkBuy { get; set; }
        public E_SupReceiveMode SupReceiveMode { get; set; }
        public List<string> CharTags { get; set; }
        public List<Sku> Skus { get; set; }
        public List<SpecItem> SpecItems { get; set; }

        public string SupplierId { get; set; }
        public class Sku
        {
            public string Id { get; set; }
            public string CumCode { get; set; }
            public string BarCode { get; set; }
            public List<SpecDes> SpecDes { get; set; }
            public decimal SalePrice { get; set; }


        }
    }
}
