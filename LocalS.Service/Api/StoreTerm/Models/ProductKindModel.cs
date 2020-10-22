using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class ProductKindModel
    {
        public ProductKindModel()
        {
            this.Childs = new List<string>();
        }

        public string KindId { get; set; }

        public string Name { get; set; }

        public List<string> Childs { get; set; }

    }
}
