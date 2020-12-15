using LocalS.Service.UI;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetPrdProductInitEdit
    {
        public RetPrdProductInitEdit()
        {
            this.Skus = new List<Sku>();
            this.Kinds = new List<TreeNode>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string SpuCode { get; set; }
        public string BarCode { get; set; }

        public List<ImgSet> DisplayImgUrls { get; set; }

        public List<ImgSet> DetailsDes { get; set; }

        public string BriefDes { get; set; }

        public List<string> KindIds { get; set; }
        public List<string> CharTags { get; set; }
        public bool IsTrgVideoService { get; set; }
        public List<Sku> Skus { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public class Sku
        {
            public string Id { get; set; }
            public List<Object> SpecDes { get; set; }

            public decimal SalePrice { get; set; }

            public string BarCode { get; set; }

            public string CumCode { get; set; }

            public bool IsOffSell { get; set; }
        }

        public List<TreeNode> Kinds { get; set; }
    }
}
