using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class SpecModel
    {
        public SpecModel()
        {

            this.Value = new List<ValueModel>();
        }
           
          
        public int Id { get; set; }

        public string Name { get; set; }

        public List<ValueModel> Value { get; set; }

        public class ValueModel
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
