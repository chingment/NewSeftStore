using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.UI
{
    public class OptionNode
    {
        public string Value { get; set; }
        public string Label { get; set; }
        public List<OptionNode> Children { get; set; }
    }
}
