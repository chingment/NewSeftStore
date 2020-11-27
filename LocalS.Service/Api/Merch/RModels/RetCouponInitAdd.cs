using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetCouponInitAdd
    {
        public RetCouponInitAdd()
        {
            this.OptionsStores = new List<OptionNode>();
            this.OptionsProductKinds = new List<TreeNode>();
        }

        public List<OptionNode> OptionsStores { get; set; }
        public List<TreeNode> OptionsProductKinds { get; set; }
    }
}
