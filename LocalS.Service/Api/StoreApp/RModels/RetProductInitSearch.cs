using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{

    public class Option
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }

    public class RetProductInitSearch
    {
        public RetProductInitSearch()
        {
            this.Condition_Kinds = new List<Option>();
        }

        public List<Option> Condition_Kinds { get; set; }
    }
}
