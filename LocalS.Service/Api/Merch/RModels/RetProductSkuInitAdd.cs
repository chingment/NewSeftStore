﻿using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetProductSkuInitAdd
    {
        public RetProductSkuInitAdd()
        {
            this.Kinds = new List<TreeNode>();
            this.Subjects = new List<TreeNode>();
        }

        public List<TreeNode> Kinds { get; set; }
        public List<TreeNode> Subjects { get; set; }

    }
}
