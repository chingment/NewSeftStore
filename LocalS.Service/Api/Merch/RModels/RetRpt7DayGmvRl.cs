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
            this.Days = new List<Rpt7DayGmvRlModel>();
        }

        public List<Rpt7DayGmvRlModel> Days { get; set; }
    }
}
