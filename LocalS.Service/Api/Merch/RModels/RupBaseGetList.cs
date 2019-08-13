using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupBaseGetList
    {
        public RupBaseGetList()
        {
            this.Limit = 10;
        }

        public int Page { get; set; }

        public int Limit { get; set; }
    }
}
