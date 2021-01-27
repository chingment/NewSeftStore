using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetRpt7DayGmvRl
    {
        public RetRpt7DayGmvRl()
        {
            this.Days = new List<object>();
        }

        public List<object> Days { get; set; }
    }
}
