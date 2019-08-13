using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RupLoginLogGetList: RupBaseGetList
    {
        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }
}
