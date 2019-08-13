using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.UI
{
    public class TreeNode
    {
        public TreeNode()
        {
            // this.Children = new List<TreeNode>();
        }

        public string Id { get; set; }
        public string PId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public object ExtAttr { get; set; }
        public string CustomLabel{ get; set; }
        public List<TreeNode> Children { get; set; }
    }
}
