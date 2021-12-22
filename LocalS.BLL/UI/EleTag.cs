using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.UI
{
    public class EleTag
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public EleTag()
        {

        }

        public EleTag(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }
}
