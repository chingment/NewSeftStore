using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupSenvivGetDayReports : RupBaseGetList
    {
        public string[] HealthDate { get; set; }

        public string Name { get; set; }

        public string UserId { get; set; }
        public string RptType { get; set; }
    }
}
