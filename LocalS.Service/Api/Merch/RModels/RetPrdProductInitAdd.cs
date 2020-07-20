using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetPrdProductInitAdd
    {
        public RetPrdProductInitAdd()
        {
            this.Kinds = new List<TreeNode>();
        }

        public List<TreeNode> Kinds { get; set; }

    }
}
