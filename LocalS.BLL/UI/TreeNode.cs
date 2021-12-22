using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.UI
{
    public class TreeNode
    {
        public TreeNode()
        {
            this.Children = new List<TreeNode>();
        }

        public string Id { get; set; }
        public string PId { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public int Depth { get; set; }
        public object ExtAttr { get; set; }
        public bool IsDisabled { get; set; }
        public List<TreeNode> Children { get; set; }
    }
}
