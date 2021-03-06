﻿using LocalS.Service.UI;
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
            this.Subjects = new List<TreeNode>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string BarCode { get; set; }

        public List<ImgSet> DispalyImgUrls { get; set; }

        public string DetailsDes { get; set; }

        public string BriefDes { get; set; }

        public List<string> KindIds { get; set; }

        public List<string> SubjectIds
        {
            get; set;
        }

        public List<Sku> Skus { get; set; }
        public class Sku
        {
            public string Id { get; set; }
            public string SpecDes { get; set; }

            public decimal SalePrice { get; set; }
        }

        public List<TreeNode> Kinds { get; set; }
        public List<TreeNode> Subjects { get; set; }
    }
}
