using LocalS.Service.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetCouponInitEdit
    {
        public RetCouponInitEdit()
        {
            this.Coupon = new CouponModel();
            this.OptionsStores = new List<OptionNode>();
            this.OptionsKinds = new List<TreeNode>();
            this.OptionsMemberLevels = new List<OptionNode>();
        }

        public CouponModel Coupon { get; set; }
        public List<OptionNode> OptionsStores { get; set; }
        public List<TreeNode> OptionsKinds { get; set; }

        public List<OptionNode> OptionsMemberLevels { get; set; }
    }
}
